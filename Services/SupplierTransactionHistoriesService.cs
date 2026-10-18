using BusinessSolution.Dtos.SupplierTransactionHistory;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class SupplierTransactionHistoriesService(ISupplierTransactionHistoriesRepository supplierTransactionHistoriesRepository) : ISupplierTransactionHistoriesService
    {
        private readonly ISupplierTransactionHistoriesRepository supplierTransactionHistoriesRepository = supplierTransactionHistoriesRepository;

        public async Task<Result<PagedResult<SupplierTransactionHistoryResponseDto>>> GetAll(SupplierTransactionHistorySearchDto searchDto)
        {
            return await supplierTransactionHistoriesRepository.GetAll(searchDto);
        }

        public async Task<Result<SupplierTransactionHistoryResponseDto>> GetById(long id)
        {
            return await supplierTransactionHistoriesRepository.GetById(id);
        }

        public async Task<Result<long>> Create(SupplierTransactionHistoryRequestDto dto)
        {
            return await supplierTransactionHistoriesRepository.Create(dto);
        }
    }
}
