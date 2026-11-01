using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerOrder
{
    public class CustomerOrderSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

