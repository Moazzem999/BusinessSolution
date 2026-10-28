namespace BusinessSolution.Dtos.SupplierPayment
{
    public class SupplierPaymentRequestDto
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public decimal Payment { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public decimal SupplierPreviousBalance { get; set; }
        public decimal SupplierNewBalance { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}

