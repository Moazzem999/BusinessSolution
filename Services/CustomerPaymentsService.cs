using BusinessSolution.Dtos.CustomerPayment;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class CustomerPaymentsService(ICustomerPaymentsRepository customerPaymentsRepository) : ICustomerPaymentsService
    {
        private readonly ICustomerPaymentsRepository customerPaymentsRepository = customerPaymentsRepository;

        public async Task<Result<PagedResult<CustomerPaymentResponseDto>>> GetAll(CustomerPaymentSearchDto searchDto)
        {
            return await customerPaymentsRepository.GetAll(searchDto);
        }

        public async Task<Result<CustomerPaymentResponseDto>> GetById(long id)
        {
            return await customerPaymentsRepository.GetById(id);
        }

        public async Task<Result<long>> Create(CustomerPaymentRequestDto dto)
        {
            return await customerPaymentsRepository.Create(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await customerPaymentsRepository.Delete(id);
        }
    }
}

