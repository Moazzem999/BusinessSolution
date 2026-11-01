using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerTransactionHistory
{
    public class CustomerTransactionHistoryResponseDto : BaseDto
    {
        public long CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerShopName { get; set; }
        public long? CustomerOrderId { get; set; }
        public string? CustomerOrderNo { get; set; }
        public long? CustomerPaymentId { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Payment { get; set; }
        public decimal CustomerPreviousBalance { get; set; }
        public decimal CustomerNewBalance { get; set; }
    }
}

