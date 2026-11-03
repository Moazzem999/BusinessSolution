using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<CustomerCountResponseDto>> TotalSpecialCustomerCount()
        {
            var count = await context.Customers.AsNoTracking()
                .CountAsync(x => x.Status == Status.Active && x.IsSpecial);

            var response = new CustomerCountResponseDto(count);
            return await Result<CustomerCountResponseDto>.SuccessAsync("Total special customer count retrieved successfully.", response);
        }

        public async Task<Result<CustomerCountResponseDto>> TotalRegularCustomerCount()
        {
            var count = await context.Customers.AsNoTracking()
                .CountAsync(x => x.Status == Status.Active && !x.IsSpecial);

            var response = new CustomerCountResponseDto(count);
            return await Result<CustomerCountResponseDto>.SuccessAsync("Total regular customer count retrieved successfully.", response);
        }

        public async Task<Result<TotalAmountResponseDto>> TotalSalesToday()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalSales = await context.CustomerOrders.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.OrderDate >= today && x.OrderDate < tomorrow)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0m;

            var response = new TotalAmountResponseDto(totalSales);
            return await Result<TotalAmountResponseDto>.SuccessAsync("Total sales today retrieved successfully.", response);
        }

        public async Task<Result<TotalAmountResponseDto>> TotalPaymentReceivedToday()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalPayment = await context.CustomerPayments.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.CreatedOn >= today && x.CreatedOn < tomorrow)
                .SumAsync(x => (decimal?)x.Payment) ?? 0m;

            var response = new TotalAmountResponseDto(totalPayment);
            return await Result<TotalAmountResponseDto>.SuccessAsync("Total payment received today retrieved successfully.", response);
        }
    }
}
