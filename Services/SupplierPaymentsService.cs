using BusinessSolution.Dtos.SupplierPayment;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class SupplierPaymentsService(ISupplierPaymentsRepository supplierPaymentsRepository) : ISupplierPaymentsService
    {
        private readonly ISupplierPaymentsRepository supplierPaymentsRepository = supplierPaymentsRepository;

        public async Task<Result<PagedResult<SupplierPaymentResponseDto>>> GetAll(SupplierPaymentSearchDto searchDto)
        {
            return await supplierPaymentsRepository.GetAll(searchDto);
        }

        public async Task<Result<SupplierPaymentResponseDto>> GetById(long id)
        {
            return await supplierPaymentsRepository.GetById(id);
        }

        public async Task<Result<long>> Create(SupplierPaymentRequestDto dto)
        {
            return await supplierPaymentsRepository.Create(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await supplierPaymentsRepository.Delete(id);
        }
    }
}

