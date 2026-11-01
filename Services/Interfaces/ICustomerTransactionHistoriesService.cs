using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ICustomerTransactionHistoriesService
    {
        Task<Result<PagedResult<CustomerTransactionHistoryResponseDto>>> GetAll(CustomerTransactionHistorySearchDto searchDto);
        Task<Result<CustomerTransactionHistoryResponseDto>> GetById(long id);
        Task<Result<long>> Create(CustomerTransactionHistoryRequestDto dto);
    }
}

