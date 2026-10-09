using BusinessSolution.Dtos.Supplier;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ISuppliersService
    {
        Task<Result<PagedResult<SupplierResponseDto>>> GetAll(SupplierSearchDto searchDto);
        Task<Result<SupplierResponseDto>> GetById(long id);
        Task<Result<List<SupplierResponseDto>>> GetByName(string name);
        Task<Result<long>> Create(SupplierRequestDto dto);
        Task<Result<SupplierResponseDto>> Update(SupplierRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
