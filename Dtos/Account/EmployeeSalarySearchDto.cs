using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Account
{
    public class EmployeeSalarySearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? EmployeeId { get; set; }
    }
}
