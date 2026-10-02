using BusinessSolution.Dtos.Employee;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IEmployeesRepository
    {
        Task<Result<List<EmployeeResponseDto>>> GetAll();
        Task<Result<EmployeeResponseDto>> GetById(long id);
        Task<Result<long>> Create(EmployeeRequestDto dto);
        Task<Result<EmployeeResponseDto>> Update(EmployeeRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
