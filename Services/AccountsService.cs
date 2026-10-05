using BusinessSolution.Dtos.Account;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class AccountsService(IAccountsRepository accountsRepository) : IAccountsService
    {
        private readonly IAccountsRepository accountsRepository = accountsRepository;

        public async Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAllEmployeeAdvancePayments(EmployeeAdvancePaymentSearchDto searchDto)
        {
            return await accountsRepository.GetAllEmployeeAdvancePayments(searchDto);
        }

        public async Task<Result<long>> CreateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto)
        {
            return await accountsRepository.CreateEmployeeAdvancePayment(dto);
        }
    }
}
