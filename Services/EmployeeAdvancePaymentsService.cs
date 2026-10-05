using BusinessSolution.Dtos.Account;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class EmployeeAdvancePaymentsService(IEmployeeAdvancePaymentsRepository employeeAdvancePaymentsRepository) : IEmployeeAdvancePaymentsService
    {
        private readonly IEmployeeAdvancePaymentsRepository employeeAdvancePaymentsRepository = employeeAdvancePaymentsRepository;

        public async Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAllEmployeeAdvancePayments(EmployeeAdvancePaymentSearchDto searchDto)
        {
            return await employeeAdvancePaymentsRepository.GetAllEmployeeAdvancePayments(searchDto);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> GetById(long id)
        {
            return await employeeAdvancePaymentsRepository.GetById(id);
        }

        public async Task<Result<long>> CreateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto)
        {
            return await employeeAdvancePaymentsRepository.CreateEmployeeAdvancePayment(dto);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> UpdateEmployeeAdvancePayment(EmployeeAdvancePaymentRequestDto dto)
        {
            return await employeeAdvancePaymentsRepository.UpdateEmployeeAdvancePayment(dto);
        }

        public async Task<Result<bool>> DeleteEmployeeAdvancePayment(long id)
        {
            return await employeeAdvancePaymentsRepository.DeleteEmployeeAdvancePayment(id);
        }
    }
}
