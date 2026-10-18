namespace BusinessSolution.Dtos.SupplierTransactionHistory
{
    public class SupplierTransactionHistoryRequestDto
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public long? SupplierOrderId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public decimal Payment { get; set; }
        public decimal SupplierPreviousBalance { get; set; }
        public decimal SupplierNewBalance { get; set; }
    }
}
