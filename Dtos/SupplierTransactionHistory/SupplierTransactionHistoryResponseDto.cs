using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierTransactionHistory
{
    public class SupplierTransactionHistoryResponseDto : BaseDto
    {
        public long SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierShopName { get; set; }
        public long? SupplierOrderId { get; set; }
        public string? SupplierOrderNo { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Payment { get; set; }
        public decimal SupplierPreviousBalance { get; set; }
        public decimal SupplierNewBalance { get; set; }
    }
}
