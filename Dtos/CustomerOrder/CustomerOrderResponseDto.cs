using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.CustomerOrder
{
    public class CustomerOrderResponseDto : BaseDto
    {
        public long CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerShopName { get; set; }
        public bool IsSpecial { get; set; } = false;
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal CustomerPreviousBalance { get; set; }
        public decimal CustomerNewBalance { get; set; }
        public List<CustomerOrderDetailResponseDto> OrderDetails { get; set; } = [];
    }
}

