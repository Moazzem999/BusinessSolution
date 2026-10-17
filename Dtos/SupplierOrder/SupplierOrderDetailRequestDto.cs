namespace BusinessSolution.Dtos.SupplierOrder
{
    public class SupplierOrderDetailRequestDto
    {
        public long Id { get; set; }
        public long SupplierOrderId { get; set; }
        public long ProductId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
