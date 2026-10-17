using BusinessSolution.Dtos.SupplierOrder;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class SupplierOrdersService(ISupplierOrdersRepository supplierOrdersRepository) : ISupplierOrdersService
    {
        private readonly ISupplierOrdersRepository supplierOrdersRepository = supplierOrdersRepository;

        public async Task<Result<PagedResult<SupplierOrderResponseDto>>> GetAll(SupplierOrderSearchDto searchDto)
        {
            return await supplierOrdersRepository.GetAll(searchDto);
        }

        public async Task<Result<SupplierOrderResponseDto>> GetById(long id)
        {
            return await supplierOrdersRepository.GetById(id);
        }

        public async Task<Result<long>> Create(SupplierOrderRequestDto dto)
        {
            return await supplierOrdersRepository.Create(dto);
        }

        public async Task<Result<SupplierOrderResponseDto>> Update(SupplierOrderRequestDto dto)
        {
            return await supplierOrdersRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await supplierOrdersRepository.Delete(id);
        }
    }
}
