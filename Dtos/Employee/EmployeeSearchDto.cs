using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Employee
{
    public class EmployeeSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}
