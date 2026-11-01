namespace BusinessSolution.Dtos.CustomerPayment
{
    public class CustomerPaymentRequestDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public decimal Payment { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public decimal CustomerPreviousBalance { get; set; }
        public decimal CustomerNewBalance { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}

