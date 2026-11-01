using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerPayment
{
    public class CustomerPaymentResponseDto : BaseDto
    {
        public long CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerShopName { get; set; }
        public decimal Payment { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string? TransactionNo { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? CustomerPreviousBalance { get; set; }
        public decimal? CustomerNewBalance { get; set; }
    }
}

