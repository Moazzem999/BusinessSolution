using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.Account
{
    public class EmployeeAdvancePaymentSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
