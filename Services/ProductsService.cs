using BusinessSolution.Dtos.Product;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class ProductsService(IProductsRepository productsRepository) : IProductsService
    {
        private readonly IProductsRepository productsRepository = productsRepository;

        public async Task<Result<PagedResult<ProductResponseDto>>> GetAll(ProductSearchDto searchDto)
        {
            return await productsRepository.GetAll(searchDto);
        }

        public async Task<Result<ProductResponseDto>> GetById(long id)
        {
            return await productsRepository.GetById(id);
        }

        public async Task<Result<List<ProductResponseDto>>> GetByName(string name)
        {
            return await productsRepository.GetByName(name);
        }

        public async Task<Result<long>> Create(ProductRequestDto dto)
        {
            return await productsRepository.Create(dto);
        }

        public async Task<Result<ProductResponseDto>> Update(ProductRequestDto dto)
        {
            return await productsRepository.Update(dto);
        }

        public async Task<Result<bool>> Delete(long id)
        {
            return await productsRepository.Delete(id);
        }
    }
}
