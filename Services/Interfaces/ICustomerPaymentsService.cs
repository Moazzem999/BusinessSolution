using BusinessSolution.Dtos.CustomerPayment;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface ICustomerPaymentsService
    {
        Task<Result<PagedResult<CustomerPaymentResponseDto>>> GetAll(CustomerPaymentSearchDto searchDto);
        Task<Result<CustomerPaymentResponseDto>> GetById(long id);
        Task<Result<long>> Create(CustomerPaymentRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}

