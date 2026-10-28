using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierPayment
{
    public class SupplierPaymentResponseDto : BaseDto
    {
        public long SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierShopName { get; set; }
        public decimal Payment { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string? TransactionNo { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? SupplierPreviousBalance { get; set; }
        public decimal? SupplierNewBalance { get; set; }
    }
}

