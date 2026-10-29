using BusinessSolution.Dtos.Customer;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class CustomersService(ICustomersRepository customersRepository) : ICustomersService
    {
        private readonly ICustomersRepository customersRepository = customersRepository;

        public async Task<Result<PagedResult<CustomerResponseDto>>> GetAll(CustomerSearchDto searchDto)
        {
            return await customersRepository.GetAll(searchDto);
        }

        public async Task<Result<CustomerResponseDto>> GetById(long id)
        {
            return await customersRepository.GetById(id);
        }

        public async Task<Result<List<CustomerResponseDto>>> GetByName(string name)
        {
            return await customersRepository.GetByName(name);
        }

        public async Task<Result<long>> Create(CustomerRequestDto dto)
        {
            return await customersRepository.Create(dto);
        }

        public async Task<Result<CustomerResponseDto>> Update(CustomerRequestDto dto)
        {
            return await customersRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await customersRepository.Delete(id);
        }
    }
}

