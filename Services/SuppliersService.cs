using BusinessSolution.Dtos.Supplier;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class SuppliersService(ISuppliersRepository suppliersRepository) : ISuppliersService
    {
        private readonly ISuppliersRepository suppliersRepository = suppliersRepository;

        public async Task<Result<PagedResult<SupplierResponseDto>>> GetAll(SupplierSearchDto searchDto)
        {
            return await suppliersRepository.GetAll(searchDto);
        }

        public async Task<Result<SupplierResponseDto>> GetById(long id)
        {
            return await suppliersRepository.GetById(id);
        }

        public async Task<Result<List<SupplierResponseDto>>> GetByName(string name)
        {
            return await suppliersRepository.GetByName(name);
        }

        public async Task<Result<long>> Create(SupplierRequestDto dto)
        {
            return await suppliersRepository.Create(dto);
        }

        public async Task<Result<SupplierResponseDto>> Update(SupplierRequestDto dto)
        {
            return await suppliersRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await suppliersRepository.Delete(id);
        }
    }
}
