namespace BusinessSolution.Dtos.SupplierOrder
{
    public class SupplierOrderRequestDto
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal SupplierPreviousBalance { get; set; }
        public decimal SupplierNewBalance { get; set; }
        public List<SupplierOrderDetailRequestDto> OrderDetails { get; set; } = [];
    }
}
