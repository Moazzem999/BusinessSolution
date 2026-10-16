using BusinessSolution.Dtos.Product;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<Result<PagedResult<ProductResponseDto>>> GetAll(ProductSearchDto searchDto);
        Task<Result<ProductResponseDto>> GetById(long id);
        Task<Result<List<ProductResponseDto>>> GetByName(string name);
        Task<Result<long>> Create(ProductRequestDto dto);
        Task<Result<ProductResponseDto>> Update(ProductRequestDto dto);
        Task<Result<bool>> Delete(long id);
    }
}
