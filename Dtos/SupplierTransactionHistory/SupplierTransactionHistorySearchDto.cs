using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierTransactionHistory
{
    public class SupplierTransactionHistorySearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? SupplierId { get; set; }
        public long? SupplierOrderId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
