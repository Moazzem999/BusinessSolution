using BusinessSolution.Dtos.Customer;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface ICustomersRepository
    {
        Task<Result<PagedResult<CustomerResponseDto>>> GetAll(CustomerSearchDto searchDto);
        Task<Result<CustomerResponseDto>> GetById(long id);
        Task<Result<List<CustomerResponseDto>>> GetByName(string name);
        Task<Result<long>> Create(CustomerRequestDto dto);
        Task<Result<CustomerResponseDto>> Update(CustomerRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}

