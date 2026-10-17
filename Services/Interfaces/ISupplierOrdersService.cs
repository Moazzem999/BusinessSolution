using BusinessSolution.Dtos.SupplierOrder;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ISupplierOrdersService
    {
        Task<Result<PagedResult<SupplierOrderResponseDto>>> GetAll(SupplierOrderSearchDto searchDto);
        Task<Result<SupplierOrderResponseDto>> GetById(long id);
        Task<Result<long>> Create(SupplierOrderRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
