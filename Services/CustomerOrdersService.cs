using BusinessSolution.Dtos.CustomerOrder;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class CustomerOrdersService(ICustomerOrdersRepository customerOrdersRepository) : ICustomerOrdersService
    {
        private readonly ICustomerOrdersRepository customerOrdersRepository = customerOrdersRepository;

        public async Task<Result<PagedResult<CustomerOrderResponseDto>>> GetAll(CustomerOrderSearchDto searchDto)
        {
            return await customerOrdersRepository.GetAll(searchDto);
        }

        public async Task<Result<CustomerOrderResponseDto>> GetById(long id)
        {
            return await customerOrdersRepository.GetById(id);
        }

        public async Task<Result<long>> Create(CustomerOrderRequestDto dto)
        {
            return await customerOrdersRepository.Create(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await customerOrdersRepository.Delete(id);
        }
    }
}

