using BusinessSolution.Dtos.Dashboard;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<Result<CustomerCountResponseDto>> TotalSpecialCustomerCount();
        Task<Result<CustomerCountResponseDto>> TotalRegularCustomerCount();
    }
}

