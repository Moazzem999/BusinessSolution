using BusinessSolution.Dtos.Account;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IAccountsService
    {
        Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAllEmployeeAdvancePayments(EmployeeAdvancePaymentSearchDto searchDto);
        Task<Result<long>> CreateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto);
    }
}
