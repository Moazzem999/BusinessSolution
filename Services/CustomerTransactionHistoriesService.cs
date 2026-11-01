using BusinessSolution.Dtos.CustomerTransactionHistory;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class CustomerTransactionHistoriesService(ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository) : ICustomerTransactionHistoriesService
    {
        private readonly ICustomerTransactionHistoriesRepository customerTransactionHistoriesRepository = customerTransactionHistoriesRepository;

        public async Task<Result<PagedResult<CustomerTransactionHistoryResponseDto>>> GetAll(CustomerTransactionHistorySearchDto searchDto)
        {
            return await customerTransactionHistoriesRepository.GetAll(searchDto);
        }

        public async Task<Result<CustomerTransactionHistoryResponseDto>> GetById(long id)
        {
            return await customerTransactionHistoriesRepository.GetById(id);
        }

        public async Task<Result<long>> Create(CustomerTransactionHistoryRequestDto dto)
        {
            return await customerTransactionHistoriesRepository.Create(dto);
        }
    }
}

