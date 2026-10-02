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
    public class EmployeesRepository(AppDbContext context) : IEmployeesRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<List<EmployeeResponseDto>>> GetAll()
        {
            var data = await context.Employees.AsNoTracking()
                .Where(x => x.Status == Status.Active)
                .ToListAsync();

            var employees = data.Select(MapToDto).ToList();

            return await Result<List<EmployeeResponseDto>>.SuccessAsync("Employees retrieved successfully.", employees);
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
                ImagePath = dto.ImagePath,
                NidImagePath = dto.NidImagePath,
            };

            context.Employees.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Employee successfully created.", newEntity.Id);
        }

        public async Task<Result<EmployeeResponseDto>> Update(EmployeeRequestDto dto)
        {
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
            entity.ImagePath = dto.ImagePath;
            entity.NidImagePath = dto.NidImagePath;

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
