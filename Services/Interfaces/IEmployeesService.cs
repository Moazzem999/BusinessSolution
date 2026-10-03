using BusinessSolution.Dtos.Employee;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IEmployeesService
    {
        Task<Result<PagedResult<EmployeeResponseDto>>> GetAll(int pageNumber = 1, int pageSize = 10);
        Task<Result<EmployeeResponseDto>> GetById(long id);
        Task<Result<long>> Create(EmployeeRequestDto dto);
        Task<Result<EmployeeResponseDto>> Update(EmployeeRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
