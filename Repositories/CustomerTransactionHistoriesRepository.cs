using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class CustomerTransactionHistoriesRepository(AppDbContext context) : ICustomerTransactionHistoriesRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<CustomerTransactionHistoryResponseDto>>> GetAll(CustomerTransactionHistorySearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.CustomerTransactionHistories.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.CustomerOrder)
                .Include(x => x.CustomerPayment)
                .Where(x => x.Status == Status.Active);

            if (searchDto.CustomerId.HasValue && searchDto.CustomerId.Value > 0)
            {
                query = query.Where(x => x.CustomerId == searchDto.CustomerId.Value);
            }

            if (searchDto.CustomerOrderId.HasValue && searchDto.CustomerOrderId.Value > 0)
            {
                query = query.Where(x => x.CustomerOrderId == searchDto.CustomerOrderId.Value);
            }

            if (searchDto.CustomerPaymentId.HasValue && searchDto.CustomerPaymentId.Value > 0)
            {
                query = query.Where(x => x.CustomerPaymentId == searchDto.CustomerPaymentId.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(x => x.TransactionDate >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                var toDate = searchDto.ToDate.Value;
                if (toDate.TimeOfDay == TimeSpan.Zero)
                {
                    toDate = toDate.Date.AddDays(1).AddTicks(-1);
                }
                query = query.Where(x => x.TransactionDate <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var amountValue);

                query = query.Where(x =>
                    x.TransactionNo.Contains(searchTerm) ||
                    (x.Customer != null && x.Customer.Name.Contains(searchTerm)) ||
                    (x.Customer != null && x.Customer.ShopName.Contains(searchTerm)) ||
                    (x.CustomerOrder != null && x.CustomerOrder.OrderNo.Contains(searchTerm)) ||
                    (x.CustomerPayment != null && x.CustomerPayment.Remarks.Contains(searchTerm)) ||
                    (isDecimal && (x.Amount == amountValue || x.Payment == amountValue || x.CustomerNewBalance == amountValue || x.CustomerPreviousBalance == amountValue)) ||
                    x.Amount.ToString().Contains(searchTerm) ||
                    x.Payment.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = data.Select(MapToResponseDto).ToList();
            var pagedResult = new PagedResult<CustomerTransactionHistoryResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<CustomerTransactionHistoryResponseDto>>.SuccessAsync("Customer transaction histories retrieved successfully.", pagedResult);
        }

        public async Task<Result<CustomerTransactionHistoryResponseDto>> GetById(long id)
        {
            var entity = await context.CustomerTransactionHistories.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.CustomerOrder)
                .Include(x => x.CustomerPayment)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<CustomerTransactionHistoryResponseDto>.RecordNotFoundAsync("Customer transaction history not found.");
            }

            return await Result<CustomerTransactionHistoryResponseDto>.SuccessAsync("Customer transaction history retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(CustomerTransactionHistoryRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.CustomerId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid customer.");
            }

            var customer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId && x.Status == Status.Active);

            if (customer == null)
            {
                return await Result<long>.RecordNotFoundAsync("Customer not found.");
            }

            if (dto.CustomerOrderId.HasValue && dto.CustomerOrderId.Value > 0)
            {
                var orderExists = await context.CustomerOrders.AsNoTracking()
                    .AnyAsync(x => x.Id == dto.CustomerOrderId.Value && x.CustomerId == dto.CustomerId && x.Status == Status.Active);

                if (!orderExists)
                {
                    return await Result<long>.BadRequestAsync("The specified customer order was not found for this customer.");
                }
            }

            if (dto.CustomerPaymentId.HasValue && dto.CustomerPaymentId.Value > 0)
            {
                var paymentExists = await context.CustomerPayments.AsNoTracking()
                    .AnyAsync(x => x.Id == dto.CustomerPaymentId.Value && x.CustomerId == dto.CustomerId && x.Status == Status.Active);

                if (!paymentExists)
                {
                    return await Result<long>.BadRequestAsync("The specified customer payment was not found for this customer.");
                }
            }

            var transactionNo = Helper.GenerateInvoiceNumber();

            var previousBalance = dto.CustomerPreviousBalance != 0 ? dto.CustomerPreviousBalance : customer.CurrentBalance;
            var newBalance = dto.CustomerNewBalance != 0 ? dto.CustomerNewBalance : (previousBalance + dto.Amount - dto.Payment);

            var hasExistingTransaction = context.Database.CurrentTransaction != null;
            var transaction = hasExistingTransaction ? null : await context.Database.BeginTransactionAsync();
            try
            {
                var historyEntity = new CustomerTransactionHistoryEntity
                {
                    CustomerId = dto.CustomerId,
                    CustomerOrderId = dto.CustomerOrderId > 0 ? dto.CustomerOrderId : null,
                    CustomerPaymentId = dto.CustomerPaymentId > 0 ? dto.CustomerPaymentId : null,
                    TransactionNo = transactionNo,
                    TransactionDate = dto.TransactionDate == default ? DateTime.Now : dto.TransactionDate,
                    Amount = dto.Amount,
                    Payment = dto.Payment,
                    CustomerPreviousBalance = previousBalance,
                    CustomerNewBalance = newBalance,
                    Status = Status.Active
                };

                context.CustomerTransactionHistories.Add(historyEntity);
                await context.SaveChangesAsync();

                if (transaction != null)
                {
                    await transaction.CommitAsync();
                }

                return await Result<long>.SuccessAsync("Customer transaction history created successfully.", historyEntity.Id);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                return await Result<long>.ErrorAsync($"Failed to create customer transaction history: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }

        private static CustomerTransactionHistoryResponseDto MapToResponseDto(CustomerTransactionHistoryEntity x)
        {
            return new CustomerTransactionHistoryResponseDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.Name,
                CustomerShopName = x.Customer?.ShopName,
                IsSpecial = x.Customer?.IsSpecial ?? false,
                CustomerOrderId = x.CustomerOrderId,
                CustomerOrderNo = x.CustomerOrder?.OrderNo,
                CustomerPaymentId = x.CustomerPaymentId,
                TransactionNo = x.TransactionNo,
                TransactionDate = x.TransactionDate,
                Amount = x.Amount,
                Payment = x.Payment,
                CustomerPreviousBalance = x.CustomerPreviousBalance,
                CustomerNewBalance = x.CustomerNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}

