using BusinessSolution.Dtos.CustomerPayment;
using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class CustomerPaymentsRepository(
        AppDbContext context,
        ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository) : ICustomerPaymentsRepository
    {
        private readonly AppDbContext context = context;
        private readonly ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository = customerTransactionHistoriesRepository;

        public async Task<Result<PagedResult<CustomerPaymentResponseDto>>> GetAll(CustomerPaymentSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.CustomerPayments.AsNoTracking()
                .Include(x => x.Customer)
                .Where(x => x.Status == Status.Active);

            if (searchDto.CustomerId.HasValue && searchDto.CustomerId.Value > 0)
            {
                query = query.Where(x => x.CustomerId == searchDto.CustomerId.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedOn >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                var toDate = searchDto.ToDate.Value;
                if (toDate.TimeOfDay == TimeSpan.Zero)
                {
                    toDate = toDate.Date.AddDays(1).AddTicks(-1);
                }
                query = query.Where(x => x.CreatedOn <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var amountValue);

                query = query.Where(x =>
                    (x.Customer != null && x.Customer.Name.Contains(searchTerm)) ||
                    (x.Customer != null && x.Customer.ShopName.Contains(searchTerm)) ||
                    x.Remarks.Contains(searchTerm) ||
                    (isDecimal && x.Payment == amountValue) ||
                    x.Payment.ToString().Contains(searchTerm) ||
                    context.CustomerTransactionHistories.Any(t => t.CustomerPaymentId == x.Id && t.Status == Status.Active && t.TransactionNo.Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paymentIds = data.Select(x => x.Id).ToList();
            var histories = await context.CustomerTransactionHistories.AsNoTracking()
                .Where(x => x.CustomerPaymentId.HasValue && paymentIds.Contains(x.CustomerPaymentId.Value) && x.Status == Status.Active)
                .ToDictionaryAsync(x => x.CustomerPaymentId!.Value);

            var resultList = data.Select(x =>
            {
                histories.TryGetValue(x.Id, out var history);
                return MapToResponseDto(x, history);
            }).ToList();

            var pagedResult = new PagedResult<CustomerPaymentResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<CustomerPaymentResponseDto>>.SuccessAsync("Customer payments retrieved successfully.", pagedResult);
        }

        public async Task<Result<CustomerPaymentResponseDto>> GetById(long id)
        {
            var entity = await context.CustomerPayments.AsNoTracking()
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<CustomerPaymentResponseDto>.RecordNotFoundAsync("Customer payment not found.");
            }

            var history = await context.CustomerTransactionHistories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerPaymentId == id && x.Status == Status.Active);

            return await Result<CustomerPaymentResponseDto>.SuccessAsync("Customer payment retrieved successfully.", MapToResponseDto(entity, history));
        }

        public async Task<Result<long>> Create(CustomerPaymentRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.CustomerId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid customer.");
            }

            if (dto.Payment <= 0)
            {
                return await Result<long>.BadRequestAsync("Payment amount must be greater than zero.");
            }

            var customer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId && x.Status == Status.Active);

            if (customer == null)
            {
                return await Result<long>.RecordNotFoundAsync("Customer not found.");
            }

            var previousBalance = dto.CustomerPreviousBalance != 0 ? dto.CustomerPreviousBalance : customer.CurrentBalance;
            var newBalance = dto.CustomerNewBalance != 0 ? dto.CustomerNewBalance : (previousBalance - dto.Payment);

            var hasExistingTransaction = context.Database.CurrentTransaction != null;
            var transaction = hasExistingTransaction ? null : await context.Database.BeginTransactionAsync();
            try
            {
                var paymentEntity = new CustomerPaymentEntity
                {
                    CustomerId = dto.CustomerId,
                    Payment = dto.Payment,
                    Remarks = dto.Remarks ?? string.Empty,
                    Status = Status.Active
                };

                context.CustomerPayments.Add(paymentEntity);
                await context.SaveChangesAsync();

                var historyDto = new CustomerTransactionHistoryRequestDto
                {
                    CustomerId = paymentEntity.CustomerId,
                    CustomerPaymentId = paymentEntity.Id,
                    TransactionDate = dto.PaymentDate ?? DateTime.Now,
                    Amount = 0,
                    Payment = paymentEntity.Payment,
                    CustomerPreviousBalance = previousBalance,
                    CustomerNewBalance = newBalance
                };

                var historyResult = await customerTransactionHistoriesRepository.Create(historyDto);
                if (!historyResult.Succeeded)
                {
                    if (transaction != null) await transaction.RollbackAsync();
                    return await Result<long>.ErrorAsync($"Failed to create customer transaction history: {historyResult.Message}", historyResult.StatusCode);
                }

                customer.CurrentBalance = newBalance;
                context.Customers.Update(customer);
                await context.SaveChangesAsync();

                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                return await Result<long>.SuccessAsync("Customer payment created successfully.", paymentEntity.Id);
            }
            catch (Exception ex)
            {
                if (transaction != null) await transaction.RollbackAsync();
                return await Result<long>.ErrorAsync($"Failed to create customer payment: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var payment = await context.CustomerPayments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (payment == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Customer payment not found.");
            }

            var customer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == payment.CustomerId && x.Status == Status.Active);

            if (customer == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Customer not found.");
            }

            // A payment can only be deleted if createdOn is within 2 days
            var isWithinTwoDays = payment.CreatedOn.Date.AddDays(2) >= DateTime.Today
                || payment.CreatedOn.AddDays(2) >= DateTime.Now;

            if (!isWithinTwoDays)
            {
                return await Result<bool>.BadRequestAsync(
                    "Cannot delete customer payment because it can only be deleted within 2 days of creation.");
            }

            // Find all active transactions linked to this customer payment
            var linkedTransactions = await context.CustomerTransactionHistories
                .Where(x => x.CustomerPaymentId == id && x.Status == Status.Active)
                .ToListAsync();

            if (linkedTransactions.Count > 1)
            {
                return await Result<bool>.BadRequestAsync(
                    "Cannot delete payment because multiple active transactions are linked to it. Please void or delete the linked transactions first.");
            }

            var paymentTransaction = linkedTransactions.FirstOrDefault();

            // Retrieve all active transactions for this customer ordered by ledger sequence (Id)
            var allActiveTransactions = await context.CustomerTransactionHistories
                .Where(x => x.CustomerId == payment.CustomerId && x.Status == Status.Active)
                .OrderBy(x => x.Id)
                .ToListAsync();

            // Determine if there are subsequent active transactions after this payment's transaction
            var subsequentTransactions = paymentTransaction != null
                ? allActiveTransactions.Where(x => x.Id > paymentTransaction.Id).OrderBy(x => x.Id).ToList()
                : [];

            bool hasNoSubsequentTransactions = subsequentTransactions.Count == 0;

            var hasExistingTransaction = context.Database.CurrentTransaction != null;
            var transaction = hasExistingTransaction ? null : await context.Database.BeginTransactionAsync();
            try
            {
                // Mark the CustomerPayments record as deleted
                payment.Status = Status.Deleted;
                context.CustomerPayments.Update(payment);

                // Void the corresponding transaction so it no longer affects the balance
                if (paymentTransaction != null)
                {
                    paymentTransaction.Status = Status.Deleted;
                    context.CustomerTransactionHistories.Update(paymentTransaction);
                }

                if (hasNoSubsequentTransactions)
                {
                    // Case 1: When deleting the latest payment
                    // Recalculate the customer's current balance from the remaining active transactions
                    var remainingActiveTransactions = allActiveTransactions
                        .Where(x => paymentTransaction == null || x.Id != paymentTransaction.Id)
                        .OrderBy(x => x.Id)
                        .ToList();

                    if (remainingActiveTransactions.Count > 0)
                    {
                        var lastRemainingTrx = remainingActiveTransactions.Last();
                        customer.CurrentBalance = lastRemainingTrx.CustomerNewBalance;
                    }
                    else
                    {
                        customer.CurrentBalance = paymentTransaction != null
                            ? paymentTransaction.CustomerPreviousBalance
                            : (customer.CurrentBalance + payment.Payment);
                    }

                    context.Customers.Update(customer);
                }
                else
                {
                    // Case 2: When deleting an older payment
                    // Recalculate the balances of all subsequent active transactions
                    decimal runningBalance = paymentTransaction != null
                        ? paymentTransaction.CustomerPreviousBalance
                        : (customer.CurrentBalance + payment.Payment);

                    var subsequentOrderIds = subsequentTransactions
                        .Where(x => x.CustomerOrderId.HasValue && x.CustomerOrderId.Value > 0)
                        .Select(x => x.CustomerOrderId!.Value)
                        .Distinct()
                        .ToList();

                    var subsequentOrders = subsequentOrderIds.Count > 0
                        ? await context.CustomerOrders
                            .Where(x => subsequentOrderIds.Contains(x.Id) && x.Status == Status.Active)
                            .ToDictionaryAsync(x => x.Id)
                        : new Dictionary<long, CustomerOrderEntity>();

                    foreach (var trx in subsequentTransactions)
                    {
                        trx.CustomerPreviousBalance = runningBalance;
                        trx.CustomerNewBalance = runningBalance + trx.Amount - trx.Payment;
                        runningBalance = trx.CustomerNewBalance;
                        context.CustomerTransactionHistories.Update(trx);

                        // If this transaction is linked to an active CustomerOrder, update the order balance as well
                        if (trx.CustomerOrderId.HasValue && subsequentOrders.TryGetValue(trx.CustomerOrderId.Value, out var subsequentOrder))
                        {
                            subsequentOrder.CustomerPreviousBalance = trx.CustomerPreviousBalance;
                            subsequentOrder.CustomerNewBalance = trx.CustomerNewBalance;
                            context.CustomerOrders.Update(subsequentOrder);
                        }
                    }

                    // Update the customer's current outstanding balance
                    customer.CurrentBalance = runningBalance;
                    context.Customers.Update(customer);
                }

                await context.SaveChangesAsync();
                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                return await Result<bool>.SuccessAsync("Customer payment deleted successfully.", true);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                return await Result<bool>.ErrorAsync($"Failed to delete customer payment: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private static CustomerPaymentResponseDto MapToResponseDto(CustomerPaymentEntity x, CustomerTransactionHistoryEntity? history)
        {
            return new CustomerPaymentResponseDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.Name,
                CustomerShopName = x.Customer?.ShopName,
                IsSpecial = x.Customer?.IsSpecial ?? false,
                Payment = x.Payment,
                Remarks = x.Remarks,
                TransactionNo = history?.TransactionNo,
                TransactionDate = history?.TransactionDate,
                CustomerPreviousBalance = history?.CustomerPreviousBalance,
                CustomerNewBalance = history?.CustomerNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}

