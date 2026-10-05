using BusinessSolution.Dtos.Account;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class EmployeeAdvancePaymentsService(IEmployeeAdvancePaymentsRepository employeeAdvancePaymentsRepository) : IEmployeeAdvancePaymentsService
    {
        private readonly IEmployeeAdvancePaymentsRepository employeeAdvancePaymentsRepository = employeeAdvancePaymentsRepository;

        public async Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAll(EmployeeAdvancePaymentSearchDto searchDto)
        {
            return await employeeAdvancePaymentsRepository.GetAll(searchDto);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> GetById(long id)
        {
            return await employeeAdvancePaymentsRepository.GetById(id);
        }

        public async Task<Result<long>> Create(EmployeeAdvancePaymentRequestDto dto)
        {
            return await employeeAdvancePaymentsRepository.Create(dto);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> Update(EmployeeAdvancePaymentRequestDto dto)
        {
            return await employeeAdvancePaymentsRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await employeeAdvancePaymentsRepository.Delete(id);
        }
    }
}
