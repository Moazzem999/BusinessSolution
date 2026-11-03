using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        public async Task<Result<List<DayWiseTotalResponseDto>>> TotalSalesThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var salesData = await context.CustomerOrders.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.OrderDate >= startOfMonth && x.OrderDate < endOfMonth)
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            var salesByDate = salesData.ToDictionary(x => x.Date, x => x.Total);
            var maxDay = Math.Max(today.Day, salesData.Count > 0 ? salesData.Max(x => x.Date.Day) : today.Day);

            var response = new List<DayWiseTotalResponseDto>();
            for (int day = 1; day <= maxDay; day++)
            {
                var date = new DateTime(today.Year, today.Month, day);
                salesByDate.TryGetValue(date, out var total);

                response.Add(new DayWiseTotalResponseDto
                {
                    Day = date.ToString("d MMMM", CultureInfo.InvariantCulture),
                    Total = total
                });
            }

            return await Result<List<DayWiseTotalResponseDto>>.SuccessAsync("Total sales this month retrieved successfully.", response);
        }

        public async Task<Result<List<DayWiseTotalResponseDto>>> TotalPaymentReceivedThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var paymentData = await context.CustomerPayments.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.CreatedOn >= startOfMonth && x.CreatedOn < endOfMonth)
                .GroupBy(x => x.CreatedOn.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(x => x.Payment)
                })
                .ToListAsync();

            var paymentByDate = paymentData.ToDictionary(x => x.Date, x => x.Total);
            var maxDay = Math.Max(today.Day, paymentData.Count > 0 ? paymentData.Max(x => x.Date.Day) : today.Day);

            var response = new List<DayWiseTotalResponseDto>();
            for (int day = 1; day <= maxDay; day++)
            {
                var date = new DateTime(today.Year, today.Month, day);
                paymentByDate.TryGetValue(date, out var total);

                response.Add(new DayWiseTotalResponseDto
                {
                    Day = date.ToString("d MMMM", CultureInfo.InvariantCulture),
                    Total = total
                });
            }

            return await Result<List<DayWiseTotalResponseDto>>.SuccessAsync("Total payment received this month retrieved successfully.", response);
        }
    }
}
