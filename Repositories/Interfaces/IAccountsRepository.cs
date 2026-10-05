using BusinessSolution.Dtos.Account;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IAccountsRepository
    {
        Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAllEmployeeAdvancePayments(EmployeeAdvancePaymentSearchDto searchDto);
        Task<Result<long>> CreateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto);
        Task<Result<EmployeeAdvancePaymentResponseDto>> UpdateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto);
        Task<Result<bool>> DeleteEmployeeAdvancePayment(long id);
    }
}
