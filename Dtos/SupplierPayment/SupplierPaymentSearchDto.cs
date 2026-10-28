using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierPayment
{
    public class SupplierPaymentSearchDto : PaginationDto
    {
        public string SearchTerm { get; set; } = string.Empty;
        public long? SupplierId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}

