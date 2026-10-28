using BusinessSolution.Dtos.SupplierPayment;
using BusinessSolution.Dtos.SupplierTransactionHistory;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class SupplierPaymentsRepository(
        AppDbContext context,
        ISupplierTransactionHistoriesRepository supplierTransactionHistoriesRepository) : ISupplierPaymentsRepository
    {
        private readonly AppDbContext context = context;
        private readonly ISupplierTransactionHistoriesRepository supplierTransactionHistoriesRepository = supplierTransactionHistoriesRepository;

        public async Task<Result<PagedResult<SupplierPaymentResponseDto>>> GetAll(SupplierPaymentSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.SupplierPayments.AsNoTracking()
                .Include(x => x.Supplier)
                .Where(x => x.Status == Status.Active);

            if (searchDto.SupplierId.HasValue && searchDto.SupplierId.Value > 0)
            {
                query = query.Where(x => x.SupplierId == searchDto.SupplierId.Value);
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
                    (x.Supplier != null && x.Supplier.Name.Contains(searchTerm)) ||
                    (x.Supplier != null && x.Supplier.ShopName.Contains(searchTerm)) ||
                    x.Remarks.Contains(searchTerm) ||
                    (isDecimal && x.Payment == amountValue) ||
                    x.Payment.ToString().Contains(searchTerm) ||
                    context.SupplierTransactionHistories.Any(t => t.SupplierPaymentId == x.Id && t.Status == Status.Active && t.TransactionNo.Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paymentIds = data.Select(x => x.Id).ToList();
            var histories = await context.SupplierTransactionHistories.AsNoTracking()
                .Where(x => x.SupplierPaymentId.HasValue && paymentIds.Contains(x.SupplierPaymentId.Value) && x.Status == Status.Active)
                .ToDictionaryAsync(x => x.SupplierPaymentId!.Value);

            var resultList = data.Select(x =>
            {
                histories.TryGetValue(x.Id, out var history);
                return MapToResponseDto(x, history);
            }).ToList();

            var pagedResult = new PagedResult<SupplierPaymentResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<SupplierPaymentResponseDto>>.SuccessAsync("Supplier payments retrieved successfully.", pagedResult);
        }

        public async Task<Result<SupplierPaymentResponseDto>> GetById(long id)
        {
            var entity = await context.SupplierPayments.AsNoTracking()
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<SupplierPaymentResponseDto>.RecordNotFoundAsync("Supplier payment not found.");
            }

            var history = await context.SupplierTransactionHistories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.SupplierPaymentId == id && x.Status == Status.Active);

            return await Result<SupplierPaymentResponseDto>.SuccessAsync("Supplier payment retrieved successfully.", MapToResponseDto(entity, history));
        }

        public async Task<Result<long>> Create(SupplierPaymentRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.SupplierId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid supplier.");
            }

            if (dto.Payment <= 0)
            {
                return await Result<long>.BadRequestAsync("Payment amount must be greater than zero.");
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == dto.SupplierId && x.Status == Status.Active);

            if (supplier == null)
            {
                return await Result<long>.RecordNotFoundAsync("Supplier not found.");
            }

            var previousBalance = dto.SupplierPreviousBalance != 0 ? dto.SupplierPreviousBalance : supplier.CurrentBalance;
            var newBalance = dto.SupplierNewBalance != 0 ? dto.SupplierNewBalance : (previousBalance - dto.Payment);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var paymentEntity = new SupplierPaymentEntity
                {
                    SupplierId = dto.SupplierId,
                    Payment = dto.Payment,
                    Remarks = dto.Remarks ?? string.Empty,
                    Status = Status.Active
                };

                context.SupplierPayments.Add(paymentEntity);
                await context.SaveChangesAsync();

                var historyDto = new SupplierTransactionHistoryRequestDto
                {
                    SupplierId = paymentEntity.SupplierId,
                    SupplierPaymentId = paymentEntity.Id,
                    TransactionDate = dto.PaymentDate ?? DateTime.Now,
                    Amount = 0,
                    Payment = paymentEntity.Payment,
                    SupplierPreviousBalance = previousBalance,
                    SupplierNewBalance = newBalance
                };

                var historyResult = await supplierTransactionHistoriesRepository.Create(historyDto);
                if (!historyResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return await Result<long>.ErrorAsync($"Failed to create supplier transaction history: {historyResult.Message}", historyResult.StatusCode);
                }

                supplier.CurrentBalance = newBalance;
                context.Suppliers.Update(supplier);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await Result<long>.SuccessAsync("Supplier payment created successfully.", paymentEntity.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<long>.ErrorAsync($"Failed to create supplier payment: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var payment = await context.SupplierPayments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (payment == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Supplier payment not found.");
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == payment.SupplierId && x.Status == Status.Active);

            if (supplier == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Supplier not found.");
            }

            // A payment can only be deleted if createdOn is within 2 days
            var isWithinTwoDays = payment.CreatedOn.Date.AddDays(2) >= DateTime.Today
                || payment.CreatedOn.AddDays(2) >= DateTime.Now;

            if (!isWithinTwoDays)
            {
                return await Result<bool>.BadRequestAsync(
                    "Cannot delete supplier payment because it can only be deleted within 2 days of creation.");
            }

            // Find all active transactions linked to this supplier payment
            var linkedTransactions = await context.SupplierTransactionHistories
                .Where(x => x.SupplierPaymentId == id && x.Status == Status.Active)
                .ToListAsync();

            if (linkedTransactions.Count > 1)
            {
                return await Result<bool>.BadRequestAsync(
                    "Cannot delete payment because multiple active transactions are linked to it. Please void or delete the linked transactions first.");
            }

            var paymentTransaction = linkedTransactions.FirstOrDefault();

            // Retrieve all active transactions for this supplier ordered by ledger sequence (Id)
            var allActiveTransactions = await context.SupplierTransactionHistories
                .Where(x => x.SupplierId == payment.SupplierId && x.Status == Status.Active)
                .OrderBy(x => x.Id)
                .ToListAsync();

            // Determine if there are subsequent active transactions after this payment's transaction
            var subsequentTransactions = paymentTransaction != null
                ? allActiveTransactions.Where(x => x.Id > paymentTransaction.Id).OrderBy(x => x.Id).ToList()
                : [];

            bool hasNoSubsequentTransactions = subsequentTransactions.Count == 0;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Mark the SupplierPayments record as deleted
                payment.Status = Status.Deleted;
                context.SupplierPayments.Update(payment);

                // Void the corresponding transaction so it no longer affects the balance
                if (paymentTransaction != null)
                {
                    paymentTransaction.Status = Status.Deleted;
                    context.SupplierTransactionHistories.Update(paymentTransaction);
                }

                if (hasNoSubsequentTransactions)
                {
                    // Case 1: When deleting the latest payment
                    // Recalculate the supplier's current balance from the remaining active transactions
                    var remainingActiveTransactions = allActiveTransactions
                        .Where(x => paymentTransaction == null || x.Id != paymentTransaction.Id)
                        .OrderBy(x => x.Id)
                        .ToList();

                    if (remainingActiveTransactions.Count > 0)
                    {
                        var lastRemainingTrx = remainingActiveTransactions.Last();
                        supplier.CurrentBalance = lastRemainingTrx.SupplierNewBalance;
                    }
                    else
                    {
                        supplier.CurrentBalance = paymentTransaction != null
                            ? paymentTransaction.SupplierPreviousBalance
                            : (supplier.CurrentBalance + payment.Payment);
                    }

                    context.Suppliers.Update(supplier);
                }
                else
                {
                    // Case 2: When deleting an older payment
                    // Recalculate the balances of all subsequent active transactions
                    decimal runningBalance = paymentTransaction != null
                        ? paymentTransaction.SupplierPreviousBalance
                        : (supplier.CurrentBalance + payment.Payment);

                    var subsequentOrderIds = subsequentTransactions
                        .Where(x => x.SupplierOrderId.HasValue && x.SupplierOrderId.Value > 0)
                        .Select(x => x.SupplierOrderId!.Value)
                        .Distinct()
                        .ToList();

                    var subsequentOrders = subsequentOrderIds.Count > 0
                        ? await context.SupplierOrders
                            .Where(x => subsequentOrderIds.Contains(x.Id) && x.Status == Status.Active)
                            .ToDictionaryAsync(x => x.Id)
                        : new Dictionary<long, SupplierOrderEntity>();

                    foreach (var trx in subsequentTransactions)
                    {
                        trx.SupplierPreviousBalance = runningBalance;
                        trx.SupplierNewBalance = runningBalance + trx.Amount - trx.Payment;
                        runningBalance = trx.SupplierNewBalance;
                        context.SupplierTransactionHistories.Update(trx);

                        // If this transaction is linked to an active SupplierOrder, update the order balance as well
                        if (trx.SupplierOrderId.HasValue && subsequentOrders.TryGetValue(trx.SupplierOrderId.Value, out var subsequentOrder))
                        {
                            subsequentOrder.SupplierPreviousBalance = trx.SupplierPreviousBalance;
                            subsequentOrder.SupplierNewBalance = trx.SupplierNewBalance;
                            context.SupplierOrders.Update(subsequentOrder);
                        }
                    }

                    // Update the supplier's current outstanding balance
                    supplier.CurrentBalance = runningBalance;
                    context.Suppliers.Update(supplier);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await Result<bool>.SuccessAsync("Supplier payment deleted successfully.", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<bool>.ErrorAsync($"Failed to delete supplier payment: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        private static SupplierPaymentResponseDto MapToResponseDto(SupplierPaymentEntity x, SupplierTransactionHistoryEntity? history)
        {
            return new SupplierPaymentResponseDto
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                SupplierName = x.Supplier?.Name,
                SupplierShopName = x.Supplier?.ShopName,
                Payment = x.Payment,
                Remarks = x.Remarks,
                TransactionNo = history?.TransactionNo,
                TransactionDate = history?.TransactionDate,
                SupplierPreviousBalance = history?.SupplierPreviousBalance,
                SupplierNewBalance = history?.SupplierNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}

