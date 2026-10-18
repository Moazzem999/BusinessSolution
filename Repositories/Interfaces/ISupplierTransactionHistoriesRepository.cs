using BusinessSolution.Dtos.SupplierTransactionHistory;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface ISupplierTransactionHistoriesRepository
    {
        Task<Result<PagedResult<SupplierTransactionHistoryResponseDto>>> GetAll(SupplierTransactionHistorySearchDto searchDto);
        Task<Result<List<SupplierTransactionHistoryResponseDto>>> GetAllBySupplierId(long supplierId);
        Task<Result<SupplierTransactionHistoryResponseDto>> GetById(long id);
        Task<Result<long>> Create(SupplierTransactionHistoryRequestDto dto);
    }
}
