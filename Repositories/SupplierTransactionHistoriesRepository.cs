using BusinessSolution.Dtos.SupplierTransactionHistory;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class SupplierTransactionHistoriesRepository(AppDbContext context) : ISupplierTransactionHistoriesRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<SupplierTransactionHistoryResponseDto>>> GetAll(SupplierTransactionHistorySearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.SupplierTransactionHistories.AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.SupplierOrder)
                .Where(x => x.Status == Status.Active);

            if (searchDto.SupplierId.HasValue && searchDto.SupplierId.Value > 0)
            {
                query = query.Where(x => x.SupplierId == searchDto.SupplierId.Value);
            }

            if (searchDto.SupplierOrderId.HasValue && searchDto.SupplierOrderId.Value > 0)
            {
                query = query.Where(x => x.SupplierOrderId == searchDto.SupplierOrderId.Value);
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
                    (x.Supplier != null && x.Supplier.Name.Contains(searchTerm)) ||
                    (x.Supplier != null && x.Supplier.ShopName.Contains(searchTerm)) ||
                    (x.SupplierOrder != null && x.SupplierOrder.OrderNo.Contains(searchTerm)) ||
                    (isDecimal && (x.Amount == amountValue || x.Payment == amountValue || x.SupplierNewBalance == amountValue || x.SupplierPreviousBalance == amountValue)) ||
                    x.Amount.ToString().Contains(searchTerm) ||
                    x.Payment.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = data.Select(MapToResponseDto).ToList();
            var pagedResult = new PagedResult<SupplierTransactionHistoryResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<SupplierTransactionHistoryResponseDto>>.SuccessAsync("Supplier transaction histories retrieved successfully.", pagedResult);
        }

        public async Task<Result<SupplierTransactionHistoryResponseDto>> GetById(long id)
        {
            var entity = await context.SupplierTransactionHistories.AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.SupplierOrder)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<SupplierTransactionHistoryResponseDto>.RecordNotFoundAsync("Supplier transaction history not found.");
            }

            return await Result<SupplierTransactionHistoryResponseDto>.SuccessAsync("Supplier transaction history retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(SupplierTransactionHistoryRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.SupplierId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid supplier.");
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == dto.SupplierId && x.Status == Status.Active);

            if (supplier == null)
            {
                return await Result<long>.RecordNotFoundAsync("Supplier not found.");
            }

            if (dto.SupplierOrderId.HasValue && dto.SupplierOrderId.Value > 0)
            {
                var orderExists = await context.SupplierOrders.AsNoTracking()
                    .AnyAsync(x => x.Id == dto.SupplierOrderId.Value && x.SupplierId == dto.SupplierId && x.Status == Status.Active);

                if (!orderExists)
                {
                    return await Result<long>.BadRequestAsync("The specified supplier order was not found for this supplier.");
                }
            }

            var transactionNo = Helper.GenerateInvoiceNumber();

            var previousBalance = dto.SupplierPreviousBalance != 0 ? dto.SupplierPreviousBalance : supplier.CurrentBalance;
            var newBalance = dto.SupplierNewBalance != 0 ? dto.SupplierNewBalance : (previousBalance + dto.Amount - dto.Payment);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var historyEntity = new SupplierTransactionHistoryEntity
                {
                    SupplierId = dto.SupplierId,
                    SupplierOrderId = dto.SupplierOrderId > 0 ? dto.SupplierOrderId : null,
                    TransactionNo = transactionNo,
                    TransactionDate = dto.TransactionDate == default ? DateTime.Now : dto.TransactionDate,
                    Amount = dto.Amount,
                    Payment = dto.Payment,
                    SupplierPreviousBalance = previousBalance,
                    SupplierNewBalance = newBalance,
                    Status = Status.Active
                };

                context.SupplierTransactionHistories.Add(historyEntity);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await Result<long>.SuccessAsync("Supplier transaction history created successfully.", historyEntity.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<long>.ErrorAsync($"Failed to create supplier transaction history: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        private static SupplierTransactionHistoryResponseDto MapToResponseDto(SupplierTransactionHistoryEntity x)
        {
            return new SupplierTransactionHistoryResponseDto
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                SupplierName = x.Supplier?.Name,
                SupplierShopName = x.Supplier?.ShopName,
                SupplierOrderId = x.SupplierOrderId,
                SupplierOrderNo = x.SupplierOrder?.OrderNo,
                TransactionNo = x.TransactionNo,
                TransactionDate = x.TransactionDate,
                Amount = x.Amount,
                Payment = x.Payment,
                SupplierPreviousBalance = x.SupplierPreviousBalance,
                SupplierNewBalance = x.SupplierNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
