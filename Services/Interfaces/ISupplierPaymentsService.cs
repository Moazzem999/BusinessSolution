using BusinessSolution.Dtos.SupplierPayment;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ISupplierPaymentsService
    {
        Task<Result<PagedResult<SupplierPaymentResponseDto>>> GetAll(SupplierPaymentSearchDto searchDto);
        Task<Result<SupplierPaymentResponseDto>> GetById(long id);
        Task<Result<long>> Create(SupplierPaymentRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}

