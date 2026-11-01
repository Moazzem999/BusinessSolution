using BusinessSolution.Dtos.CustomerOrder;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ICustomerOrdersService
    {
        Task<Result<PagedResult<CustomerOrderResponseDto>>> GetAll(CustomerOrderSearchDto searchDto);
        Task<Result<CustomerOrderResponseDto>> GetById(long id);
        Task<Result<long>> Create(CustomerOrderRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}

