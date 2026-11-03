using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class DashboardService(IDashboardRepository dashboardRepository) : IDashboardService
    {
        private readonly IDashboardRepository dashboardRepository = dashboardRepository;

        public async Task<Result<CustomerCountResponseDto>> TotalSpecialCustomerCount()
        {
            return await dashboardRepository.TotalSpecialCustomerCount();
        }

        public async Task<Result<CustomerCountResponseDto>> TotalRegularCustomerCount()
        {
            return await dashboardRepository.TotalRegularCustomerCount();
        }
    }
}

