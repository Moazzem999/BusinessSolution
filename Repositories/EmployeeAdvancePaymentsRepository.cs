using BusinessSolution.Dtos.Account;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class EmployeeAdvancePaymentsRepository(AppDbContext context) : IEmployeeAdvancePaymentsRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAll(EmployeeAdvancePaymentSearchDto searchDto)
        {
            var pageNumber = searchDto.PageNumber;
            var pageSize = searchDto.PageSize;

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = context.EmployeeAdvancePayments.AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.Status == Status.Active);

            if (searchDto.EmployeeId.HasValue && searchDto.EmployeeId.Value > 0)
            {
                query = query.Where(x => x.EmployeeId == searchDto.EmployeeId.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(x => x.PaymentDate >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                var toDate = searchDto.ToDate.Value;
                if (toDate.TimeOfDay == TimeSpan.Zero)
                {
                    toDate = toDate.Date.AddDays(1).AddTicks(-1);
                }
                query = query.Where(x => x.PaymentDate <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.Trim();
                bool isDecimal = decimal.TryParse(searchTerm, out var amountValue);

                query = query.Where(x =>
                    (x.Employee != null && x.Employee.Name.Contains(searchTerm)) ||
                    x.Description.Contains(searchTerm) ||
                    (isDecimal && x.Amount == amountValue) ||
                    x.Amount.ToString().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = data.Select(MapToResponseDto).ToList();
            var pagedResult = new PagedResult<EmployeeAdvancePaymentResponseDto>(resultList, totalCount, pageNumber, pageSize);

            return await Result<PagedResult<EmployeeAdvancePaymentResponseDto>>.SuccessAsync("Employee advance payments retrieved successfully.", pagedResult);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> GetById(long id)
        {
            var entity = await context.EmployeeAdvancePayments.AsNoTracking()
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.RecordNotFoundAsync("Employee advance payment record not found.");
            }

            return await Result<EmployeeAdvancePaymentResponseDto>.SuccessAsync("Employee advance payment retrieved successfully.", MapToResponseDto(entity));
        }

        public async Task<Result<long>> Create(EmployeeAdvancePaymentRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<long>.BadRequestAsync("Invalid request data.");
            }

            if (dto.EmployeeId <= 0)
            {
                return await Result<long>.BadRequestAsync("Please select a valid employee.");
            }

            if (dto.Amount <= 0)
            {
                return await Result<long>.BadRequestAsync("Amount must be greater than zero.");
            }

            var employeeExists = await context.Employees.AsNoTracking()
                .AnyAsync(x => x.Id == dto.EmployeeId && x.Status == Status.Active);

            if (!employeeExists)
            {
                return await Result<long>.RecordNotFoundAsync("Employee not found.");
            }

            var newEntity = new EmployeeAdvancePaymentEntity
            {
                EmployeeId = dto.EmployeeId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                Description = dto.Description ?? string.Empty
            };

            context.EmployeeAdvancePayments.Add(newEntity);
            await context.SaveChangesAsync();

            return await Result<long>.SuccessAsync("Employee advance payment successfully created.", newEntity.Id);
        }

        public async Task<Result<EmployeeAdvancePaymentResponseDto>> Update(EmployeeAdvancePaymentRequestDto dto)
        {
            if (dto == null)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.BadRequestAsync("Invalid request data.");
            }

            if (dto.Id <= 0)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.BadRequestAsync("Invalid payment record ID.");
            }

            var entity = await context.EmployeeAdvancePayments
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.RecordNotFoundAsync("Employee advance payment record not found.");
            }

            if (dto.EmployeeId <= 0)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.BadRequestAsync("Please select a valid employee.");
            }

            if (dto.Amount <= 0)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.BadRequestAsync("Amount must be greater than zero.");
            }

            var employee = await context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == dto.EmployeeId && x.Status == Status.Active);

            if (employee == null)
            {
                return await Result<EmployeeAdvancePaymentResponseDto>.RecordNotFoundAsync("Employee not found.");
            }

            entity.EmployeeId = dto.EmployeeId;
            entity.Amount = dto.Amount;
            entity.PaymentDate = dto.PaymentDate;
            entity.Description = dto.Description ?? string.Empty;

            context.EmployeeAdvancePayments.Update(entity);
            await context.SaveChangesAsync();

            entity.Employee = employee;
            return await Result<EmployeeAdvancePaymentResponseDto>.SuccessAsync("Employee advance payment successfully updated.", MapToResponseDto(entity));
        }

        public async Task<Result<bool>> Delete(long id)
        {
            var entity = await context.EmployeeAdvancePayments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == Status.Active);

            if (entity == null)
            {
                return await Result<bool>.RecordNotFoundAsync("Employee advance payment record not found.");
            }

            entity.Status = Status.Deleted;

            context.EmployeeAdvancePayments.Update(entity);
            await context.SaveChangesAsync();

            return await Result<bool>.SuccessAsync("Employee advance payment successfully deleted.", true);
        }

        private static EmployeeAdvancePaymentResponseDto MapToResponseDto(EmployeeAdvancePaymentEntity x)
        {
            return new EmployeeAdvancePaymentResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee?.Name,
                Amount = x.Amount,
                PaymentDate = x.PaymentDate,
                Description = x.Description,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                Status = x.Status
            };
        }
    }
}
