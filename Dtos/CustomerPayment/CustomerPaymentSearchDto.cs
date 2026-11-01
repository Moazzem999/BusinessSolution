using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerPayment
{
    public class CustomerPaymentSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

