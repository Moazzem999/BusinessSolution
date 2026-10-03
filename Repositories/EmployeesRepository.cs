using BusinessSolution.Dtos.Employee;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class EmployeesRepository(AppDbContext context, IWebHostEnvironment environment) : IEmployeesRepository
    {
        private readonly AppDbContext context = context;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<Result<PagedResult<EmployeeResponseDto>>> GetAll(EmployeeSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.Employees.AsNoTracking()
                .Where(x => x.Status == Status.Active);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var salaryValue);

                query = query.Where(x =>
                    x.Name.Contains(searchTerm) ||
                    x.NidNo.Contains(searchTerm) ||
                    x.Mobile.Contains(searchTerm) ||
                    x.Designation.Contains(searchTerm) ||
                    (isDecimal && x.Salary == salaryValue) ||
                    x.Salary.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var employees = data.Select(MapToDto).ToList();
            var pagedResult = new PagedResult<EmployeeResponseDto>(employees, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<EmployeeResponseDto>>.SuccessAsync("Employees retrieved successfully.", pagedResult);
        }

        public async Task<Result<EmployeeResponseDto>> GetById(long id)
        {
            var entity = await context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeResponseDto>.RecordNotFoundAsync($"Employee not found.");
            }

            return await Result<EmployeeResponseDto>.SuccessAsync("Employee retrieved successfully.", MapToDto(entity));
        }

        public async Task<Result<long>> Create(EmployeeRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<long>.BadRequestAsync("Employee name is required.");
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

                imagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Employees");
            }

            string? nidImagePath = null;
            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<long>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<long>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                nidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Employees");
            }

            var newEntity = new EmployeeEntity
            {
                Name = dto.Name,
                FatherName = dto.FatherName,
                NidNo = dto.NidNo,
                Email = dto.Email,
                Mobile = dto.Mobile,
                DateOfBirth = dto.DateOfBirth,
                MaritalStatus = dto.MaritalStatus,
                Religion = dto.Religion,
                Designation = dto.Designation,
                AcademicQualification = dto.AcademicQualification,
                PresentAddress = dto.PresentAddress,
                PermanentAddress = dto.PermanentAddress,
                JoiningDate = dto.JoiningDate,
                Salary = dto.Salary,
                ImagePath = imagePath,
                NidImagePath = nidImagePath,
            };

            context.Employees.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Employee successfully created.", newEntity.Id);
        }

        public async Task<Result<EmployeeResponseDto>> Update(EmployeeRequestDto dto)
        {
            var maxFileSize = 300 * 1024; // 300 KB
            string[] allowedFileTypes = [".jpg", ".jpeg", ".png"];

            if (dto == null)
            {
                return await Result<EmployeeResponseDto>.BadRequestAsync("Invalid request data.");
            }

            var entity = await context.Employees
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeResponseDto>.RecordNotFoundAsync($"Employee not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return await Result<EmployeeResponseDto>.BadRequestAsync("Employee name is required.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !Helper.IsValidEmail(dto.Email))
            {
                return await Result<EmployeeResponseDto>.BadRequestAsync("Please provide a valid email address.");
            }

            if (dto.Image != null)
            {
                var fileType = Path.GetExtension(dto.Image.FileName).ToLower();

                if (dto.Image.Length > maxFileSize)
                    return await Result<EmployeeResponseDto>.BadRequestAsync("Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<EmployeeResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.ImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.ImagePath);
                }
                entity.ImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.Image, "Employees");
            }

            if (dto.NidImage != null)
            {
                var fileType = Path.GetExtension(dto.NidImage.FileName).ToLower();

                if (dto.NidImage.Length > maxFileSize)
                    return await Result<EmployeeResponseDto>.BadRequestAsync("Nid Image size must be less than 300 KB.");

                if (string.IsNullOrEmpty(fileType) || !allowedFileTypes.Contains(fileType))
                    return await Result<EmployeeResponseDto>.BadRequestAsync("Invalid file type. Allowed types: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(entity.NidImagePath))
                {
                    Helper.DeleteImage(environment.ContentRootPath, entity.NidImagePath);
                }
                entity.NidImagePath = await Helper.SaveImage(environment.ContentRootPath, dto.NidImage, "Employees");
            }

            entity.Name = dto.Name;
            entity.FatherName = dto.FatherName;
            entity.NidNo = dto.NidNo;
            entity.Email = dto.Email;
            entity.Mobile = dto.Mobile;
            entity.DateOfBirth = dto.DateOfBirth;
            entity.MaritalStatus = dto.MaritalStatus;
            entity.Religion = dto.Religion;
            entity.Designation = dto.Designation;
            entity.AcademicQualification = dto.AcademicQualification;
            entity.PresentAddress = dto.PresentAddress;
            entity.PermanentAddress = dto.PermanentAddress;
            entity.JoiningDate = dto.JoiningDate;
            entity.Salary = dto.Salary;
            entity.ImagePath = entity.ImagePath;
            entity.NidImagePath = entity.NidImagePath;

            context.Employees.Update(entity);
            await context.SaveChangesAsync();

            return await Result<EmployeeResponseDto>.SuccessAsync("Employee successfully updated.", MapToDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.Employees
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync($"Employee not found.");
            }

            entity.Status = Status.Deleted;

            context.Employees.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Employee successfully deleted.", true);
        }

        private static EmployeeResponseDto MapToDto(EmployeeEntity x)
        {
            return new EmployeeResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                FatherName = x.FatherName,
                NidNo = x.NidNo,
                Email = x.Email,
                Mobile = x.Mobile,
                DateOfBirth = x.DateOfBirth,
                MaritalStatus = x.MaritalStatus,
                Religion = x.Religion,
                Designation = x.Designation,
                AcademicQualification = x.AcademicQualification,
                PresentAddress = x.PresentAddress,
                PermanentAddress = x.PermanentAddress,
                JoiningDate = x.JoiningDate,
                Salary = x.Salary,
                ImagePath = x.ImagePath,
                NidImagePath = x.NidImagePath,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
