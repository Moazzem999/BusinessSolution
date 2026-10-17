using BusinessSolution.Dtos.Common;

namespace BusinessSolution.Dtos.SupplierOrder
{
    public class SupplierOrderResponseDto : BaseDto
    {
        public long SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierShopName { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public decimal SupplierPreviousBalance { get; set; }
        public decimal SupplierNewBalance { get; set; }
        public List<SupplierOrderDetailResponseDto> OrderDetails { get; set; } = [];
    }
}
