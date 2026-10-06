using BusinessSolution.Dtos.Account;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IEmployeeSalariesRepository
    {
        Task<Result<PagedResult<EmployeeSalaryResponseDto>>> GetAll(EmployeeSalarySearchDto searchDto);
        Task<Result<EmployeeSalaryResponseDto>> GetById(long id);
        Task<Result<long>> Create(EmployeeSalaryRequestDto dto);
        Task<Result<EmployeeSalaryResponseDto>> Update(EmployeeSalaryRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
