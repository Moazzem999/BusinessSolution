using BusinessSolution.Dtos.Customer;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class CustomersRepository(AppDbContext context, IWebHostEnvironment environment) : ICustomersRepository
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<Result<PagedResult<CustomerResponseDto>>> GetAll(CustomerSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.Customers.AsNoTracking()
                .Where(x => x.Status == Status.Active);

            if (searchDto.IsSpecial.HasValue)
            {
                query = query.Where(x => x.IsSpecial == searchDto.IsSpecial.Value);
            }

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

            var customers = data.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<CustomerResponseDto>(customers, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<CustomerResponseDto>>.SuccessAsync("Customers retrieved successfully.", pagedResult);
        }

        public async Task<Result<CustomerResponseDto>> GetById(long id)
        {
            var entity = await context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<CustomerResponseDto>.RecordNotFoundAsync("Customer not found.");
            }

            return await Result<CustomerResponseDto>.SuccessAsync("Customer retrieved successfully.", MapToDto(entity));
        }

        public async Task<Result<List<CustomerResponseDto>>> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return await Result<List<CustomerResponseDto>>.SuccessAsync("Customers retrieved successfully.", new List<CustomerResponseDto>());
            }

            var searchTerm = name.Trim();
            var data = await context.Customers.AsNoTracking()
                .Where(x => x.Status == Status.Active
                && (x.Name.Contains(searchTerm)
                || x.ShopName.Contains(searchTerm)
                || x.Mobile.Contains(searchTerm)
                || x.Email.Contains(searchTerm)))
                .ToListAsync();

            var customers = data.Select(MapToDto).ToList();
            return await Result<List<CustomerResponseDto>>.SuccessAsync("Customers retrieved successfully.", customers);
        }

        public async Task<Result<long>> Create(CustomerRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<long>.BadRequestAsync("Customer name is required.");
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

                imagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Customers");
            }

            string? nidImagePath = null;
            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                nidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Customers");
            }

            string? chequeImagePath = null;
            if (dto.ChequeImage != null)
            {
                var fileType = Path.GetExtension(dto.ChequeImage.FileName).ToLower();

                if (dto.ChequeImage.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Cheque Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                chequeImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.ChequeImage, "Customers");
            }

            var newEntity = new CustomerEntity
            {
                Name = dto.Name,
                ShopName = dto.ShopName ?? string.Empty,
                NidNo = dto.NidNo ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                Mobile = dto.Mobile ?? string.Empty,
                IsSpecial = dto.IsSpecial,
                AdditionalDetails = dto.AdditionalDetails ?? string.Empty,
                PresentAddress = dto.PresentAddress ?? string.Empty,
                PermanentAddress = dto.PermanentAddress ?? string.Empty,
                CurrentBalance = dto.CurrentBalance,
                ImagePath = imagePath,
                NidImagePath = nidImagePath,
                ChequeImagePath = chequeImagePath
            };

            context.Customers.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Customer successfully created.", newEntity.Id);
        }

        public async Task<Result<CustomerResponseDto>> Update(CustomerRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<CustomerResponseDto>.BadRequestAsync("Invalid request data.");
            }

            var entity = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<CustomerResponseDto>.RecordNotFoundAsync("Customer not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<CustomerResponseDto>.BadRequestAsync("Customer name is required.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !Helper.IsValidEmail(dto.Email))
            {
                return await Result<CustomerResponseDto>.BadRequestAsync("Please provide a valid email address.");
            }

            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<CustomerResponseDto>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<CustomerResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ImagePath);
                }
                entity.ImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Customers");
            }

            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<CustomerResponseDto>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<CustomerResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.NidImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.NidImagePath);
                }
                entity.NidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Customers");
            }

            if (dto.ChequeImage != null)
            {
                var fileType = Path.GetExtension(dto.ChequeImage.FileName).ToLower();

                if (dto.ChequeImage.Length > maxFileSize)
                    return await Result<CustomerResponseDto>.BadRequestAsync("Cheque Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<CustomerResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ChequeImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ChequeImagePath);
                }
                entity.ChequeImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.ChequeImage, "Customers");
            }

            entity.Name = dto.Name;
            entity.ShopName = dto.ShopName ?? string.Empty;
            entity.NidNo = dto.NidNo ?? string.Empty;
            entity.Email = dto.Email ?? string.Empty;
            entity.Mobile = dto.Mobile ?? string.Empty;
            entity.IsSpecial = dto.IsSpecial;
            entity.AdditionalDetails = dto.AdditionalDetails ?? string.Empty;
            entity.PresentAddress = dto.PresentAddress ?? string.Empty;
            entity.PermanentAddress = dto.PermanentAddress ?? string.Empty;
            entity.CurrentBalance = dto.CurrentBalance;

            context.Customers.Update(entity);
            await context.SaveChangesAsync();

            return await Result<CustomerResponseDto>.SuccessAsync("Customer successfully updated.", MapToDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.Customers
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Customer not found.");
            }

            entity.Status = Status.Deleted;

            context.Customers.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Customer successfully deleted.", true);
        }

        private static CustomerResponseDto MapToDto(CustomerEntity x)
        {
            return new CustomerResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                ShopName = x.ShopName,
                NidNo = x.NidNo,
                Email = x.Email,
                Mobile = x.Mobile,
                IsSpecial = x.IsSpecial,
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

