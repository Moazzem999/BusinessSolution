using BusinessSolution.Dtos.CustomerOrder;
using BusinessSolution.Dtos.CustomerPayment;
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
    public class CustomerOrdersRepository(
        AppDbContext context,
        ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository,
        ICustomerPaymentsRepository customerPaymentsRepository) : ICustomerOrdersRepository
    {
        private readonly AppDbContext context = context;
        private readonly ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository = customerTransactionHistoriesRepository;
        private readonly ICustomerPaymentsRepository customerPaymentsRepository = customerPaymentsRepository;

        public async Task<Result<PagedResult<CustomerOrderResponseDto>>> GetAll(CustomerOrderSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.CustomerOrders.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.OrderDetails.Where(d => d.Status == Status.Active))
                    .ThenInclude(d => d.Product)
                .Where(x => x.Status == Status.Active);

            if (searchDto.CustomerId.HasValue && searchDto.CustomerId.Value > 0)
            {
                query = query.Where(x => x.CustomerId == searchDto.CustomerId.Value);
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
                    (x.Customer != null && x.Customer.Name.Contains(searchTerm)) ||
                    (x.Customer != null && x.Customer.ShopName.Contains(searchTerm)) ||
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
            var pagedResult = new PagedResult<CustomerOrderResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<CustomerOrderResponseDto>>.SuccessAsync("Customer orders retrieved successfully.", pagedResult);
        }

        public async Task<Result<CustomerOrderResponseDto>> GetById(long id)
        {
            var entity = await context.CustomerOrders.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.OrderDetails.Where(d => d.Status == Status.Active))
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<CustomerOrderResponseDto>.RecordNotFoundAsync("Customer order not found.");
            }

            return await Result<CustomerOrderResponseDto>.SuccessAsync("Customer order retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(CustomerOrderRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.CustomerId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid customer.");
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

            var customer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId && x.Status == Status.Active);

            if (customer == null)
            {
                return await Result<long>.RecordNotFoundAsync("Customer not found.");
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

            var previousBalance = dto.CustomerPreviousBalance != 0 ? dto.CustomerPreviousBalance : customer.CurrentBalance;
            var newBalance = dto.CustomerNewBalance != 0 ? dto.CustomerNewBalance : (previousBalance + totalAmount);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var orderEntity = new CustomerOrderEntity
                {
                    CustomerId = dto.CustomerId,
                    OrderNo = orderNo,
                    OrderDate = dto.OrderDate == default ? DateTime.Now : dto.OrderDate,
                    TotalAmount = totalAmount,
                    Notes = dto.Notes ?? string.Empty,
                    CustomerPreviousBalance = previousBalance,
                    CustomerNewBalance = newBalance,
                    Status = Status.Active
                };

                context.CustomerOrders.Add(orderEntity);
                await context.SaveChangesAsync();

                foreach (var item in dto.OrderDetails)
                {
                    var lineTotal = item.LineTotal > 0 ? item.LineTotal : (item.Quantity * item.UnitPrice);
                    var detailEntity = new CustomerOrderDetailsEntity
                    {
                        CustomerOrderId = orderEntity.Id,
                        ProductId = item.ProductId,
                        Remarks = item.Remarks ?? string.Empty,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = lineTotal,
                        Status = Status.Active
                    };

                    context.CustomerOrderDetails.Add(detailEntity);
                }

                await context.SaveChangesAsync();

                var historyDto = new CustomerTransactionHistoryRequestDto
                {
                    CustomerId = orderEntity.CustomerId,
                    CustomerOrderId = orderEntity.Id,
                    TransactionDate = orderEntity.OrderDate,
                    Amount = orderEntity.TotalAmount,
                    Payment = 0,
                    CustomerPreviousBalance = previousBalance,
                    CustomerNewBalance = newBalance
                };

                var historyResult = await customerTransactionHistoriesRepository.Create(historyDto);
                if (!historyResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return await Result<long>.ErrorAsync($"Failed to create customer transaction history: {historyResult.Message}", historyResult.StatusCode);
                }

                customer.CurrentBalance = newBalance;
                context.Customers.Update(customer);
                await context.SaveChangesAsync();

                if (!customer.IsSpecial && orderEntity.TotalAmount > 0)
                {
                    var paymentDto = new CustomerPaymentRequestDto
                    {
                        CustomerId = orderEntity.CustomerId,
                        Payment = orderEntity.TotalAmount,
                        Remarks = $"Payment for Order {orderEntity.OrderNo}",
                        CustomerPreviousBalance = orderEntity.CustomerNewBalance,
                        CustomerNewBalance = orderEntity.CustomerNewBalance - orderEntity.TotalAmount,
                        PaymentDate = orderEntity.OrderDate
                    };

                    var paymentResult = await customerPaymentsRepository.Create(paymentDto);
                    if (!paymentResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return await Result<long>.ErrorAsync($"Failed to create customer payment: {paymentResult.Message}", paymentResult.StatusCode);
                    }
                }

                await transaction.CommitAsync();

                return await Result<long>.SuccessAsync("Customer order created successfully.", orderEntity.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<long>.ErrorAsync($"Failed to create customer order: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var order = await context.CustomerOrders
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (order == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Customer order not found.");
            }

            var customer = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == order.CustomerId && x.Status == Status.Active);

            if (customer == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Customer not found.");
            }

            // An order can only be deleted if createdOn is within 2 days
            var isWithinTwoDays = order.CreatedOn.Date.AddDays(2) >= DateTime.Today
                || order.CreatedOn.AddDays(2) >= DateTime.Now;

            if (!isWithinTwoDays)
            {
                return await Result<bool>.BadRequestAsync(
                    $"Cannot delete order '{order.OrderNo}' because it can only be deleted within 2 days of creation.");
            }

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (!customer.IsSpecial)
                {
                    var payments = await context.CustomerPayments
                        .Where(x => x.CustomerId == order.CustomerId
                                 && x.Status == Status.Active
                                 && x.Remarks.Contains(order.OrderNo))
                        .ToListAsync();

                    foreach (var payment in payments)
                    {
                        var deletePaymentResult = await customerPaymentsRepository.Delete(payment.Id);
                        if (!deletePaymentResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return await Result<bool>.ErrorAsync($"Failed to delete customer payment: {deletePaymentResult.Message}", deletePaymentResult.StatusCode);
                        }
                    }
                }

                // Find all active transactions linked to this customer order
                var linkedTransactions = await context.CustomerTransactionHistories
                    .Where(x => x.CustomerOrderId == id && x.Status == Status.Active)
                    .ToListAsync();

                // 1. Validate whether payments or other transactions are linked to that order
                var linkedPayments = linkedTransactions.Where(x => x.Payment > 0).ToList();
                if (linkedPayments.Count > 0)
                {
                    await transaction.RollbackAsync();
                    return await Result<bool>.BadRequestAsync(
                        $"Cannot delete order '{order.OrderNo}' because one or more active payments are linked to it. Please void or delete the linked payment(s) first.");
                }

                if (linkedTransactions.Count > 1)
                {
                    await transaction.RollbackAsync();
                    return await Result<bool>.BadRequestAsync(
                        $"Cannot delete order '{order.OrderNo}' because multiple active transactions are linked to it. Please void or delete the linked transactions first.");
                }

                var orderTransaction = linkedTransactions.FirstOrDefault();

                // Retrieve all active transactions for this customer ordered by ledger sequence (Id)
                var allActiveTransactions = await context.CustomerTransactionHistories
                    .Where(x => x.CustomerId == order.CustomerId && x.Status == Status.Active)
                    .OrderBy(x => x.Id)
                    .ToListAsync();

                // Determine if there are subsequent active transactions after this order's transaction
                var subsequentTransactions = orderTransaction != null
                    ? allActiveTransactions.Where(x => x.Id > orderTransaction.Id).OrderBy(x => x.Id).ToList()
                    : [];

                bool hasNoSubsequentTransactions = subsequentTransactions.Count == 0;

                // Mark the CustomerOrders record as deleted
                order.Status = Status.Deleted;
                context.CustomerOrders.Update(order);

                // Mark its CustomerOrderDetails records as deleted
                var details = await context.CustomerOrderDetails
                    .Where(x => x.CustomerOrderId == id && x.Status == Status.Active)
                    .ToListAsync();

                foreach (var detail in details)
                {
                    detail.Status = Status.Deleted;
                    context.CustomerOrderDetails.Update(detail);
                }

                // Void the corresponding transaction so it no longer affects the balance
                if (orderTransaction != null)
                {
                    orderTransaction.Status = Status.Deleted;
                    context.CustomerTransactionHistories.Update(orderTransaction);
                }

                if (hasNoSubsequentTransactions)
                {
                    // Case 1: When deleting the latest order
                    // Recalculate the customer's current balance from the remaining active transactions
                    var remainingActiveTransactions = allActiveTransactions
                        .Where(x => orderTransaction == null || x.Id != orderTransaction.Id)
                        .OrderBy(x => x.Id)
                        .ToList();

                    if (remainingActiveTransactions.Count > 0)
                    {
                        var lastRemainingTrx = remainingActiveTransactions.Last();
                        customer.CurrentBalance = lastRemainingTrx.CustomerNewBalance;
                    }
                    else
                    {
                        customer.CurrentBalance = orderTransaction != null
                            ? orderTransaction.CustomerPreviousBalance
                            : (customer.CurrentBalance - order.TotalAmount);
                    }

                    context.Customers.Update(customer);
                }
                else
                {
                    // Case 2: When deleting an older order
                    // Recalculate the balances of all subsequent active transactions
                    decimal runningBalance = orderTransaction!.CustomerPreviousBalance;

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
                await transaction.CommitAsync();

                return await Result<bool>.SuccessAsync("Customer order deleted successfully.", true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await Result<bool>.ErrorAsync($"Failed to delete customer order: {ex.Message}", StatusCodes.Status500InternalServerError, ex);
            }
        }

        private static CustomerOrderResponseDto MapToResponseDto(CustomerOrderEntity x)
        {
            return new CustomerOrderResponseDto
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.Name,
                CustomerShopName = x.Customer?.ShopName,
                IsSpecial = x.Customer?.IsSpecial ?? false,
                OrderNo = x.OrderNo,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount,
                Notes = x.Notes,
                CustomerPreviousBalance = x.CustomerPreviousBalance,
                CustomerNewBalance = x.CustomerNewBalance,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status,
                OrderDetails = x.OrderDetails?
                    .Where(d => d.Status == Status.Active)
                    .Select(d => new CustomerOrderDetailResponseDto
                    {
                        Id = d.Id,
                        CustomerOrderId = d.CustomerOrderId,
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
            var prefix = $"CO-{currentYear}-";
            var lastOrder = await context.CustomerOrders
                .Where(x => x.OrderNo.StartsWith(prefix))
                .OrderByDescending(x => x.Id)
                .Select(x => x.OrderNo)
                .FirstOrDefaultAsync();

            int serial = 0;
            if (!string.IsNullOrEmpty(lastOrder) && lastOrder.Length > prefix.Length)
            {
                int.TryParse(lastOrder[prefix.Length..], out serial);
            }

            string orderNo = Helper.GenerateCustomerOrderNumber(serial);
            while (await context.CustomerOrders.AnyAsync(x => x.OrderNo == orderNo))
            {
                serial++;
                orderNo = Helper.GenerateCustomerOrderNumber(serial);
            }

            return orderNo;
        }
    }
}

