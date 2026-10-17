using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierOrder
{
    public class SupplierOrderDetailResponseDto : BaseDto
    {
        public long SupplierOrderId { get; set; }
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
