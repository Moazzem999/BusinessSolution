using BusinessSolution.Dtos.SupplierOrder;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class SupplierOrdersRepository(AppDbContext context) : ISupplierOrdersRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<SupplierOrderResponseDto>>> GetAll(SupplierOrderSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.SupplierOrders.AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.OrderDetails.Where(d => d.Status == Status.Active))
                    .ThenInclude(d => d.Product)
                .Where(x => x.Status == Status.Active);

            if (searchDto.SupplierId.HasValue && searchDto.SupplierId.Value > 0)
            {
                query = query.Where(x => x.SupplierId == searchDto.SupplierId.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(x => x.OrderDate >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                var toDate = searchDto.ToDate.Value;
                if (toDate.TimeOfDay == TimeSpan.Zero)
                {
                    toDate = toDate.Date.AddDays(1).AddTicks(-1);
                }
                query = query.Where(x => x.OrderDate <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var amountValue);

                query = query.Where(x =>
                    x.OrderNo.Contains(searchTerm) ||
                    (x.Supplier != null && x.Supplier.Name.Contains(searchTerm)) ||
                    (x.Supplier != null && x.Supplier.ShopName.Contains(searchTerm)) ||
                    x.Notes.Contains(searchTerm) ||
                    (isDecimal && x.TotalAmount == amountValue) ||
                    x.TotalAmount.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = data.Select(MapToResponseDto).ToList();
            var pagedResult = new PagedResult<SupplierOrderResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<SupplierOrderResponseDto>>.SuccessAsync("Supplier orders retrieved successfully.", pagedResult);
        }

        public async Task<Result<SupplierOrderResponseDto>> GetById(long id)
        {
            var entity = await context.SupplierOrders.AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.OrderDetails.Where(d => d.Status == Status.Active))
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<SupplierOrderResponseDto>.RecordNotFoundAsync("Supplier order not found.");
            }

            return await Result<SupplierOrderResponseDto>.SuccessAsync("Supplier order retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(SupplierOrderRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.SupplierId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid supplier.");
            }

            if (dto.OrderDetails == null || dto.OrderDetails.Count == 0)
            {
                return await Result<long>.BadRequestAsync("Order must contain at least one item.");
            }

            foreach (var item in dto.OrderDetails)
            {
                if (item.ProductId <= 0)
                {
                    return await Result<long>.BadRequestAsync("Each order item must specify a valid product.");
                }

                if (item.Quantity <= 0)
                {
                    return await Result<long>.BadRequestAsync("Quantity must be greater than zero for all items.");
                }

                if (item.UnitPrice < 0)
                {
                    return await Result<long>.BadRequestAsync("Unit price cannot be negative.");
                }
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == dto.SupplierId && x.Status == Status.Active);

            if (supplier == null)
            {
                return await Result<long>.RecordNotFoundAsync("Supplier not found.");
            }

            var productIds = dto.OrderDetails.Select(x => x.ProductId).Distinct().ToList();
            var validProductsCount = await context.Products
                .CountAsync(x => productIds.Contains(x.Id) && x.Status == Status.Active);

            if (validProductsCount != productIds.Count)
            {
                return await Result<long>.BadRequestAsync("One or more selected products were not found.");
            }

            var orderNo = await GenerateUniqueOrderNumberAsync();

            var calculatedTotal = dto.OrderDetails.Sum(x => x.LineTotal > 0 ? x.LineTotal : (x.Quantity * x.UnitPrice));
            var totalAmount = dto.TotalAmount > 0 ? dto.TotalAmount : calculatedTotal;

            var previousBalance = dto.SupplierPreviousBalance != 0 ? dto.SupplierPreviousBalance : supplier.CurrentBalance;
            var newBalance = dto.SupplierNewBalance != 0 ? dto.SupplierNewBalance : (previousBalance + totalAmount);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var orderEntity = new SupplierOrderEntity
                {
                    SupplierId = dto.SupplierId,
                    OrderNo = orderNo,
                    OrderDate = dto.OrderDate == default ? DateTime.Now : dto.OrderDate,
                    TotalAmount = totalAmount,
                    Notes = dto.Notes ?? string.Empty,
                    SupplierPreviousBalance = previousBalance,
                    SupplierNewBalance = newBalance,
                    Status = Status.Active
                };

                context.SupplierOrders.Add(orderEntity);
                await context.SaveChangesAsync();

                foreach (var item in dto.OrderDetails)
                {
                    var lineTotal = item.LineTotal > 0 ? item.LineTotal : (item.Quantity * item.UnitPrice);
                    var detailEntity = new SupplierOrderDetailsEntity
                    {
                        SupplierOrderId = orderEntity.Id,
                        ProductId = item.ProductId,
                        Remarks = item.Remarks ?? string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = lineTotal,
                        Status = Status.Active
                    };

                    context.SupplierOrderDetails.Add(detailEntity);
                }

                await context.SaveChangesAsync();

                supplier.CurrentBalance = newBalance;
                context.Suppliers.Update(supplier);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await Result<long>.SuccessAsync("Supplier order created successfully.", orderEntity.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<long>.ErrorAsync($"Failed to create supplier order: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        public async Task<Result<SupplierOrderResponseDto>> Update(SupplierOrderRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<SupplierOrderResponseDto>.BadRequestAsync("Invalid request data.");
            }

            if (dto.Id <= 0)
            {
                return await Result<SupplierOrderResponseDto>.BadRequestAsync("Invalid order ID.");
            }

            if (dto.SupplierId <= 0)
            {
                return await Result<SupplierOrderResponseDto>.BadRequestAsync("Please select a valid supplier.");
            }

            if (dto.OrderDetails == null || dto.OrderDetails.Count == 0)
            {
                return await Result<SupplierOrderResponseDto>.BadRequestAsync("Order must contain at least one item.");
            }

            foreach (var item in dto.OrderDetails)
            {
                if (item.ProductId <= 0)
                {
                    return await Result<SupplierOrderResponseDto>.BadRequestAsync("Each order item must specify a valid product.");
                }

                if (item.Quantity <= 0)
                {
                    return await Result<SupplierOrderResponseDto>.BadRequestAsync("Quantity must be greater than zero for all items.");
                }

                if (item.UnitPrice < 0)
                {
                    return await Result<SupplierOrderResponseDto>.BadRequestAsync("Unit price cannot be negative.");
                }
            }

            var order = await context.SupplierOrders
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (order == null)
            {
                return await Result<SupplierOrderResponseDto>.RecordNotFoundAsync("Supplier order not found.");
            }

            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == dto.SupplierId && x.Status == Status.Active);

            if (supplier == null)
            {
                return await Result<SupplierOrderResponseDto>.RecordNotFoundAsync("Supplier not found.");
            }

            var productIds = dto.OrderDetails.Select(x => x.ProductId).Distinct().ToList();
            var validProductsCount = await context.Products
                .CountAsync(x => productIds.Contains(x.Id) && x.Status == Status.Active);

            if (validProductsCount != productIds.Count)
            {
                return await Result<SupplierOrderResponseDto>.BadRequestAsync("One or more selected products were not found.");
            }



            var calculatedTotal = dto.OrderDetails.Sum(x => x.LineTotal > 0 ? x.LineTotal : (x.Quantity * x.UnitPrice));
            var newTotalAmount = dto.TotalAmount > 0 ? dto.TotalAmount : calculatedTotal;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // Revert previous balance impact on suppliers
                if (order.SupplierId != dto.SupplierId)
                {
                    var oldSupplier = await context.Suppliers
                        .FirstOrDefaultAsync(x => x.Id == order.SupplierId && x.Status == Status.Active);

                    if (oldSupplier != null)
                    {
                        oldSupplier.CurrentBalance -= order.TotalAmount;
                        context.Suppliers.Update(oldSupplier);
                    }

                    supplier.CurrentBalance += newTotalAmount;
                }
                else
                {
                    supplier.CurrentBalance = supplier.CurrentBalance - order.TotalAmount + newTotalAmount;
                }

                context.Suppliers.Update(supplier);

                var previousBalance = dto.SupplierPreviousBalance != 0 ? dto.SupplierPreviousBalance : (supplier.CurrentBalance - newTotalAmount);
                var newBalance = dto.SupplierNewBalance != 0 ? dto.SupplierNewBalance : supplier.CurrentBalance;

                order.SupplierId = dto.SupplierId;
                if (string.IsNullOrWhiteSpace(order.OrderNo))
                {
                    order.OrderNo = await GenerateUniqueOrderNumberAsync();
                }
                order.OrderDate = dto.OrderDate == default ? order.OrderDate : dto.OrderDate;
                order.TotalAmount = newTotalAmount;
                order.Notes = dto.Notes ?? string.Empty;
                order.SupplierPreviousBalance = previousBalance;
                order.SupplierNewBalance = newBalance;

                context.SupplierOrders.Update(order);

                // Sync order details
                var existingDetails = await context.SupplierOrderDetails
                    .Where(x => x.SupplierOrderId == order.Id && x.Status == Status.Active)
                    .ToListAsync();

                var requestDetailIds = dto.OrderDetails.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();

                // Delete details not included in the request
                foreach (var existing in existingDetails)
                {
                    if (!requestDetailIds.Contains(existing.Id))
                    {
                        existing.Status = Status.Deleted;
                        context.SupplierOrderDetails.Update(existing);
                    }
                }

                // Update existing or add new details
                foreach (var item in dto.OrderDetails)
                {
                    var lineTotal = item.LineTotal > 0 ? item.LineTotal : (item.Quantity * item.UnitPrice);
                    if (item.Id > 0)
                    {
                        var existing = existingDetails.FirstOrDefault(x => x.Id == item.Id);
                        if (existing != null)
                        {
                            existing.ProductId = item.ProductId;
                            existing.Remarks = item.Remarks ?? string.Empty;
                            existing.Quantity = item.Quantity;
                            existing.UnitPrice = item.UnitPrice;
                            existing.LineTotal = lineTotal;
                            context.SupplierOrderDetails.Update(existing);
                        }
                    }
                    else
                    {
                        var newDetail = new SupplierOrderDetailsEntity
                        {
                            SupplierOrderId = order.Id,
                            ProductId = item.ProductId,
                            Remarks = item.Remarks ?? string.Empty,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            LineTotal = lineTotal,
                            Status = Status.Active
                        };
                        context.SupplierOrderDetails.Add(newDetail);
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                var updatedEntity = await context.SupplierOrders.AsNoTracking()
                    .Include(x => x.Supplier)
                    .Include(x => x.OrderDetails.Where(d => d.Status == Status.Active))
                        .ThenInclude(d => d.Product)
                    .FirstOrDefaultAsync(x => x.Id == order.Id);

                return await Result<SupplierOrderResponseDto>.SuccessAsync("Supplier order updated successfully.", MapToResponseDto(updatedEntity!));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<SupplierOrderResponseDto>.ErrorAsync($"Failed to update supplier order: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var order = await context.SupplierOrders
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (order == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Supplier order not found.");
            }

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                order.Status = Status.Deleted;
                context.SupplierOrders.Update(order);

                var details = await context.SupplierOrderDetails
                    .Where(x => x.SupplierOrderId == id && x.Status == Status.Active)
                    .ToListAsync();

                foreach (var detail in details)
                {
                    detail.Status = Status.Deleted;
                    context.SupplierOrderDetails.Update(detail);
                }

                var supplier = await context.Suppliers
                    .FirstOrDefaultAsync(x => x.Id == order.SupplierId && x.Status == Status.Active);

                if (supplier != null)
                {
                    supplier.CurrentBalance -= order.TotalAmount;
                    context.Suppliers.Update(supplier);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await Result<bool>.SuccessAsync("Supplier order deleted successfully.", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<bool>.ErrorAsync($"Failed to delete supplier order: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        private static SupplierOrderResponseDto MapToResponseDto(SupplierOrderEntity x)
        {
            return new SupplierOrderResponseDto
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                SupplierName = x.Supplier?.Name,
                SupplierShopName = x.Supplier?.ShopName,
                OrderNo = x.OrderNo,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                Notes = x.Notes,
                SupplierPreviousBalance = x.SupplierPreviousBalance,
                SupplierNewBalance = x.SupplierNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status,
                OrderDetails = x.OrderDetails?
                    .Where(d => d.Status == Status.Active)
                    .Select(d => new SupplierOrderDetailResponseDto
                    {
                        Id = d.Id,
                        SupplierOrderId = d.SupplierOrderId,
                        ProductId = d.ProductId,
                        ProductName = d.Product?.Name,
                        Remarks = d.Remarks,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        LineTotal = d.LineTotal,
                        CreatedOn = d.CreatedOn,
                        UpdatedOn = d.UpdatedOn,
                        CreatedBy = d.CreatedBy,
                        UpdatedBy = d.UpdatedBy,
                        Status = d.Status
                    }).ToList() ?? []
            };
        }
        private async Task<string> GenerateUniqueOrderNumberAsync()
        {
            var currentYear = DateTime.Now.Year;
            var prefix = $"PO-{currentYear}-";
            var lastOrder = await context.SupplierOrders
                .Where(x => x.OrderNo.StartsWith(prefix))
                .OrderByDescending(x => x.Id)
                .Select(x => x.OrderNo)
                .FirstOrDefaultAsync();

            int serial = 0;
            if (!string.IsNullOrEmpty(lastOrder) && lastOrder.Length > prefix.Length)
            {
                int.TryParse(lastOrder[prefix.Length..], out serial);
            }

            string orderNo = Helper.GeneratePurchaseOrderNumber(serial);
            while (await context.SupplierOrders.AnyAsync(x => x.OrderNo == orderNo))
            {
                serial++;
                orderNo = Helper.GeneratePurchaseOrderNumber(serial);
            }

            return orderNo;
        }
    }
}
