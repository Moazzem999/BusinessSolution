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

        public async Task<Result<TotalAmountResponseDto>> TotalSalesToday()
        {
            return await dashboardRepository.TotalSalesToday();
        }

        public async Task<Result<TotalAmountResponseDto>> TotalPaymentReceivedToday()
        {
            return await dashboardRepository.TotalPaymentReceivedToday();
        }

        public async Task<Result<List<DayWiseTotalResponseDto>>> TotalSalesThisMonth()
        {
            return await dashboardRepository.TotalSalesThisMonth();
        }

        public async Task<Result<List<DayWiseTotalResponseDto>>> TotalPaymentReceivedThisMonth()
        {
            return await dashboardRepository.TotalPaymentReceivedThisMonth();
        }

        public async Task<Result<List<MonthWiseTotalResponseDto>>> TotalSalesThisYear()
        {
            return await dashboardRepository.TotalSalesThisYear();
        }

        public async Task<Result<List<MonthWiseTotalResponseDto>>> TotalPaymentReceivedThisYear()
        {
            return await dashboardRepository.TotalPaymentReceivedThisYear();
        }
    }
}
