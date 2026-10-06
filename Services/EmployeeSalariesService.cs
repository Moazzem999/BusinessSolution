using BusinessSolution.Dtos.Account;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class EmployeeSalariesService(IEmployeeSalariesRepository employeeSalariesRepository) : IEmployeeSalariesService
    {
        private readonly IEmployeeSalariesRepository employeeSalariesRepository = employeeSalariesRepository;

        public async Task<Result<PagedResult<EmployeeSalaryResponseDto>>> GetAll(EmployeeSalarySearchDto searchDto)
        {
            return await employeeSalariesRepository.GetAll(searchDto);
        }

        public async Task<Result<EmployeeSalaryResponseDto>> GetById(long id)
        {
            return await employeeSalariesRepository.GetById(id);
        }

        public async Task<Result<long>> Create(EmployeeSalaryRequestDto dto)
        {
            return await employeeSalariesRepository.Create(dto);
        }

        public async Task<Result<EmployeeSalaryResponseDto>> Update(EmployeeSalaryRequestDto dto)
        {
            return await employeeSalariesRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await employeeSalariesRepository.Delete(id);
        }
    }
}
