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

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> GetById(long id)
        {
            return await accountsRepository.GetById(id);
        }

        public async Task<Result<long>> CreateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto)
        {
            return await accountsRepository.CreateEmployeeAdvancePayment(dto);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> UpdateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto)
        {
            return await accountsRepository.UpdateEmployeeAdvancePayment(dto);
        }

        public async Task<Result<bool>> DeleteEmployeeAdvancePayment(long id)
        {
            return await accountsRepository.DeleteEmployeeAdvancePayment(id);
        }
    }
}
