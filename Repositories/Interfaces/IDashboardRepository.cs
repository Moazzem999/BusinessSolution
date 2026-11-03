using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<Result<CustomerCountResponseDto>> TotalSpecialCustomerCount();
        Task<Result<CustomerCountResponseDto>> TotalRegularCustomerCount();
        Task<Result<TotalAmountResponseDto>> TotalSalesToday();
        Task<Result<TotalAmountResponseDto>> TotalPaymentReceivedToday();
    }
}
