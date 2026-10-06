using BusinessSolution.Dtos.Account;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class EmployeeSalariesRepository(AppDbContext context) : IEmployeeSalariesRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<EmployeeSalaryResponseDto>>> GetAll(EmployeeSalarySearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.EmployeeSalaries.AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.Status == Status.Active);

            if (searchDto.EmployeeId.HasValue && searchDto.EmployeeId.Value > 0)
            {
                query = query.Where(x => x.EmployeeId == searchDto.EmployeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var decimalValue);

                query = query.Where(x =>
                    (x.Employee != null && x.Employee.Name.Contains(searchTerm)) ||
                    x.PaySlipFor.Contains(searchTerm) ||
                    x.Remarks.Contains(searchTerm) ||
                    (isDecimal && (x.Salary == decimalValue || x.AdvancePayment == decimalValue || x.Total == decimalValue))
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = data.Select(MapToResponseDto).ToList();
            var pagedResult = new PagedResult<EmployeeSalaryResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<EmployeeSalaryResponseDto>>.SuccessAsync("Employee salaries retrieved successfully.", pagedResult);
        }

        public async Task<Result<EmployeeSalaryResponseDto>> GetById(long id)
        {
            var entity = await context.EmployeeSalaries.AsNoTracking()
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeSalaryResponseDto>.RecordNotFoundAsync("Employee salary record not found.");
            }

            return await Result<EmployeeSalaryResponseDto>.SuccessAsync("Employee salary retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(EmployeeSalaryRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.EmployeeId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid employee.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaySlipFor))
            {
                return await Result<long>.BadRequestAsync("Please enter a valid Month.");
            }

            var employeeExists = await context.Employees.AsNoTracking()
                .AnyAsync(x => x.Id == dto.EmployeeId && x.Status == Status.Active);

            if (!employeeExists)
            {
                return await Result<long>.RecordNotFoundAsync("Employee not found.");
            }

            var entity = await context.EmployeeSalaries
                .FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId && x.PaySlipFor == dto.PaySlipFor && x.Status == Status.Active);

            if (entity != null)
            {
                return await Result<long>.RecordNotFoundAsync("Employee salary record already exists.");
            }

            var newEntity = new EmployeeSalaryEntity
            {
                EmployeeId = dto.EmployeeId,
                Salary = dto.Salary,
                AdvancePayment = dto.AdvancePayment,
                BonusPayment = dto.BonusPayment,
                OthersBill = dto.OthersBill,
                Total = dto.Total,
                PaySlipFor = dto.PaySlipFor ?? string.Empty,
                Remarks = dto.Remarks ?? string.Empty
            };

            context.EmployeeSalaries.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Employee salary record successfully created.", newEntity.Id);
        }

        public async Task<Result<EmployeeSalaryResponseDto>> Update(EmployeeSalaryRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<EmployeeSalaryResponseDto>.BadRequestAsync("Invalid request data.");
            }

            if (dto.Id <= 0)
            {
                return await Result<EmployeeSalaryResponseDto>.BadRequestAsync("Invalid salary record ID.");
            }

            var entity = await context.EmployeeSalaries
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeSalaryResponseDto>.RecordNotFoundAsync("Employee salary record not found.");
            }

            if (dto.EmployeeId <= 0)
            {
                return await Result<EmployeeSalaryResponseDto>.BadRequestAsync("Please select a valid employee.");
            }

            var employee = await context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.EmployeeId && x.Status == Status.Active);

            if (employee == null)
            {
                return await Result<EmployeeSalaryResponseDto>.RecordNotFoundAsync("Employee not found.");
            }

            entity.EmployeeId = dto.EmployeeId;
            entity.Salary = dto.Salary;
            entity.AdvancePayment = dto.AdvancePayment;
            entity.BonusPayment = dto.BonusPayment;
            entity.OthersBill = dto.OthersBill;
            entity.Total = dto.Total;
            entity.PaySlipFor = dto.PaySlipFor ?? string.Empty;
            entity.Remarks = dto.Remarks ?? string.Empty;

            context.EmployeeSalaries.Update(entity);
            await context.SaveChangesAsync();

            entity.Employee = employee;
            return await Result<EmployeeSalaryResponseDto>.SuccessAsync("Employee salary record successfully updated.", MapToResponseDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.EmployeeSalaries
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Employee salary record not found.");
            }

            entity.Status = Status.Deleted;

            context.EmployeeSalaries.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Employee salary record successfully deleted.", true);
        }

        private static EmployeeSalaryResponseDto MapToResponseDto(EmployeeSalaryEntity x)
        {
            return new EmployeeSalaryResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee?.Name,
                Designation = x.Employee?.Designation,
                ImagePath = x.Employee?.ImagePath,
                Salary = x.Salary,
                AdvancePayment = x.AdvancePayment,
                BonusPayment = x.BonusPayment,
                OthersBill = x.OthersBill,
                Total = x.Total,
                PaySlipFor = x.PaySlipFor,
                Remarks = x.Remarks,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
