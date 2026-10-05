using BusinessSolution.Dtos.Account;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IEmployeeAdvancePaymentsService
    {
        Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAll(EmployeeAdvancePaymentSearchDto searchDto);
        Task<Result<EmployeeAdvancePaymentResponseDto>> GetById(long id);
        Task<Result<long>> Create(EmployeeAdvancePaymentRequestDto dto);
        Task<Result<EmployeeAdvancePaymentResponseDto>> Update(EmployeeAdvancePaymentRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
