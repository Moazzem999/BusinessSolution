namespace BusinessSolution.Dtos.CustomerOrder
{
    public class CustomerOrderRequestDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal CustomerPreviousBalance { get; set; }
        public decimal CustomerNewBalance { get; set; }
        public List<CustomerOrderDetailRequestDto> OrderDetails { get; set; } = [];
    }
}

