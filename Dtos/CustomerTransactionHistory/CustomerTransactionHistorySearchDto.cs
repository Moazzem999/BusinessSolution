using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerTransactionHistory
{
    public class CustomerTransactionHistorySearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? CustomerId { get; set; }
        public long? CustomerOrderId { get; set; }
        public long? CustomerPaymentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

