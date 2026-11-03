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
    }
}

