using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface ICustomerTransactionHistoriesRepository
    {
        Task<Result<PagedResult<CustomerTransactionHistoryResponseDto>>> GetAll(CustomerTransactionHistorySearchDto searchDto);
        Task<Result<CustomerTransactionHistoryResponseDto>> GetById(long id);
        Task<Result<long>> Create(CustomerTransactionHistoryRequestDto dto);
    }
}

