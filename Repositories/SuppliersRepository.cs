using BusinessSolution.Dtos.Supplier;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class SuppliersRepository(AppDbContext context, IWebHostEnvironment environment) : ISuppliersRepository
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<Result<PagedResult<SupplierResponseDto>>> GetAll(SupplierSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.Suppliers.AsNoTracking()
                .Where(x => x.Status == Status.Active);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var balanceValue);

                query = query.Where(x =>
                    x.Name.Contains(searchTerm) ||
                    x.ShopName.Contains(searchTerm) ||
                    x.NidNo.Contains(searchTerm) ||
                    x.Mobile.Contains(searchTerm) ||
                    x.Email.Contains(searchTerm) ||
                    (isDecimal && x.CurrentBalance == balanceValue) ||
                    x.CurrentBalance.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var suppliers = data.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<SupplierResponseDto>(suppliers, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<SupplierResponseDto>>.SuccessAsync("Suppliers retrieved successfully.", pagedResult);
        }

        public async Task<Result<SupplierResponseDto>> GetById(long id)
        {
            var entity = await context.Suppliers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<SupplierResponseDto>.RecordNotFoundAsync("Supplier not found.");
            }

            return await Result<SupplierResponseDto>.SuccessAsync("Supplier retrieved successfully.", MapToDto(entity));
        }

        public async Task<Result<List<SupplierResponseDto>>> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Result<List<SupplierResponseDto>>.SuccessAsync("Suppliers retrieved successfully.", new List<SupplierResponseDto>());
            }

            var searchTerm = name.Trim();
            var data = await context.Suppliers.AsNoTracking()
                .Where(x => x.Status == Status.Active && x.Name.Contains(searchTerm))
                .ToListAsync();

            var suppliers = data.Select(MapToDto).ToList();
            return await Result<List<SupplierResponseDto>>.SuccessAsync("Suppliers retrieved successfully.", suppliers);
        }

        public async Task<Result<long>> Create(SupplierRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<long>.BadRequestAsync("Supplier name is required.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !Helper.IsValidEmail(dto.Email))
            {
                return await Result<long>.BadRequestAsync("Please provide a valid email address.");
            }

            string? imagePath = null;
            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                imagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Suppliers");
            }

            string? nidImagePath = null;
            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                nidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Suppliers");
            }

            string? chequeImagePath = null;
            if (dto.ChequeImage != null)
            {
                var fileType = Path.GetExtension(dto.ChequeImage.FileName).ToLower();

                if (dto.ChequeImage.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Cheque Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                chequeImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.ChequeImage, "Suppliers");
            }

            var newEntity = new SupplierEntity
            {
                Name = dto.Name,
                ShopName = dto.ShopName,
                NidNo = dto.NidNo ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Mobile = dto.Mobile,
                AdditionalDetails = dto.AdditionalDetails,
                PresentAddress = dto.PresentAddress,
                PermanentAddress = dto.PermanentAddress,
                CurrentBalance = dto.CurrentBalance,
                ImagePath = imagePath,
                NidImagePath = nidImagePath,
                ChequeImagePath = chequeImagePath
            };

            context.Suppliers.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Supplier successfully created.", newEntity.Id);
        }

        public async Task<Result<SupplierResponseDto>> Update(SupplierRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<SupplierResponseDto>.BadRequestAsync("Invalid request data.");
            }

            var entity = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<SupplierResponseDto>.RecordNotFoundAsync("Supplier not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<SupplierResponseDto>.BadRequestAsync("Supplier name is required.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !Helper.IsValidEmail(dto.Email))
            {
                return await Result<SupplierResponseDto>.BadRequestAsync("Please provide a valid email address.");
            }

            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<SupplierResponseDto>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<SupplierResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ImagePath);
                }
                entity.ImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Suppliers");
            }

            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<SupplierResponseDto>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<SupplierResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.NidImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.NidImagePath);
                }
                entity.NidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Suppliers");
            }

            if (dto.ChequeImage != null)
            {
                var fileType = Path.GetExtension(dto.ChequeImage.FileName).ToLower();

                if (dto.ChequeImage.Length > maxFileSize)
                    return await Result<SupplierResponseDto>.BadRequestAsync("Cheque Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<SupplierResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ChequeImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ChequeImagePath);
                }
                entity.ChequeImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.ChequeImage, "Suppliers");
            }

            entity.Name = dto.Name;
            entity.ShopName = dto.ShopName;
            entity.NidNo = dto.NidNo ?? string.Empty;
            entity.Email = dto.Email ?? string.Empty;
            entity.Mobile = dto.Mobile;
            entity.AdditionalDetails = dto.AdditionalDetails;
            entity.PresentAddress = dto.PresentAddress;
            entity.PermanentAddress = dto.PermanentAddress;
            entity.CurrentBalance = dto.CurrentBalance;

            context.Suppliers.Update(entity);
            await context.SaveChangesAsync();

            return await Result<SupplierResponseDto>.SuccessAsync("Supplier successfully updated.", MapToDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Supplier not found.");
            }

            entity.Status = Status.Deleted;

            context.Suppliers.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Supplier successfully deleted.", true);
        }

        private static SupplierResponseDto MapToDto(SupplierEntity x)
        {
            return new SupplierResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                ShopName = x.ShopName,
                NidNo = x.NidNo,
                Email = x.Email,
                Mobile = x.Mobile,
                AdditionalDetails = x.AdditionalDetails,
                PresentAddress = x.PresentAddress,
                PermanentAddress = x.PermanentAddress,
                CurrentBalance = x.CurrentBalance,
                ImagePath = x.ImagePath,
                NidImagePath = x.NidImagePath,
                ChequeImagePath = x.ChequeImagePath,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
