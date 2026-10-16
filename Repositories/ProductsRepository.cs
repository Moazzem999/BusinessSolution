using BusinessSolution.Dtos.Product;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class ProductsRepository(AppDbContext context, IWebHostEnvironment environment) : IProductsRepository
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<Result<PagedResult<ProductResponseDto>>> GetAll(ProductSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.Products.AsNoTracking()
                .Where(x => x.Status == Status.Active);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                query = query.Where(x =>
                    x.Name.Contains(searchTerm) ||
                    x.Details.Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var products = data.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<ProductResponseDto>(products, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<ProductResponseDto>>.SuccessAsync("Products retrieved successfully.", pagedResult);
        }

        public async Task<Result<ProductResponseDto>> GetById(long id)
        {
            var entity = await context.Products.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<ProductResponseDto>.RecordNotFoundAsync("Product not found.");
            }

            return await Result<ProductResponseDto>.SuccessAsync("Product retrieved successfully.", MapToDto(entity));
        }

        public async Task<Result<List<ProductResponseDto>>> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Result<List<ProductResponseDto>>.SuccessAsync("Products retrieved successfully.", new List<ProductResponseDto>());
            }

            var searchTerm = name.Trim();
            var data = await context.Products.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.Name.Contains(searchTerm))
                .ToListAsync();

            var products = data.Select(MapToDto).ToList();
            return await Result<List<ProductResponseDto>>.SuccessAsync("Products retrieved successfully.", products);
        }

        public async Task<Result<long>> Create(ProductRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<long>.BadRequestAsync("Product name is required.");
            }

            string? imagePath = null;
            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                imagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Products");
            }

            var newEntity = new ProductEntity
            {
                Name = dto.Name.Trim(),
                Details = dto.Details?.Trim() ?? string.Empty,
                ImagePath = imagePath
            };

            context.Products.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Product successfully created.", newEntity.Id);
        }

        public async Task<Result<ProductResponseDto>> Update(ProductRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<ProductResponseDto>.BadRequestAsync("Invalid request data.");
            }

            var entity = await context.Products
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<ProductResponseDto>.RecordNotFoundAsync("Product not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<ProductResponseDto>.BadRequestAsync("Product name is required.");
            }

            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<ProductResponseDto>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<ProductResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ImagePath);
                }
                entity.ImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Products");
            }

            entity.Name = dto.Name.Trim();
            entity.Details = dto.Details?.Trim() ?? string.Empty;

            context.Products.Update(entity);
            await context.SaveChangesAsync();

            return await Result<ProductResponseDto>.SuccessAsync("Product successfully updated.", MapToDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.Products
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Product not found.");
            }

            entity.Status = Status.Deleted;

            context.Products.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Product successfully deleted.", true);
        }

        private static ProductResponseDto MapToDto(ProductEntity x)
        {
            return new ProductResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Details = x.Details,
                ImagePath = x.ImagePath,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
