namespace BusinessSolution.Dtos.CustomerOrder
{
    public class CustomerOrderDetailRequestDto
    {
        public long Id { get; set; }
        public long CustomerOrderId { get; set; }
        public long ProductId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}

