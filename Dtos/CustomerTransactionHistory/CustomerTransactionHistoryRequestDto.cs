namespace BusinessSolution.Dtos.CustomerTransactionHistory
{
    public class CustomerTransactionHistoryRequestDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long? CustomerOrderId { get; set; }
        public long? CustomerPaymentId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public decimal Payment { get; set; }
        public decimal CustomerPreviousBalance { get; set; }
        public decimal CustomerNewBalance { get; set; }
    }
}

