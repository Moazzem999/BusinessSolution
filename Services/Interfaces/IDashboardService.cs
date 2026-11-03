using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<CustomerCountResponseDto>> TotalSpecialCustomerCount();
        Task<Result<CustomerCountResponseDto>> TotalRegularCustomerCount();
        Task<Result<TotalAmountResponseDto>> TotalSalesToday();
        Task<Result<TotalAmountResponseDto>> TotalPaymentReceivedToday();
        Task<Result<List<DayWiseTotalResponseDto>>> TotalSalesThisMonth();
        Task<Result<List<DayWiseTotalResponseDto>>> TotalPaymentReceivedThisMonth();
        Task<Result<List<MonthWiseTotalResponseDto>>> TotalSalesThisYear();
        Task<Result<List<MonthWiseTotalResponseDto>>> TotalPaymentReceivedThisYear();
    }
}
