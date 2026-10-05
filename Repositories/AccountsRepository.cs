using BusinessSolution.Dtos.Account;
using BusinessSolution.Entities;
using BusinessSolution.Entities.Context;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Repositories
{
    public class AccountsRepository(AppDbContext context) : IAccountsRepository
    {
        private readonly AppDbContext context = context;

        public async Task<Result<PagedResult<EmployeeAdvancePaymentResponseDto>>> GetAllEmployeeAdvancePayments(EmployeeAdvancePaymentSearchDto searchDto)
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
