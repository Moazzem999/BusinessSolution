using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("SupplierOrders")]
    public class SupplierOrderEntity : BaseEntity
    {
        public long SupplierId { get; set; }

        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SupplierPreviousBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SupplierNewBalance { get; set; }


        [ForeignKey(nameof(SupplierId))]
        public SupplierEntity? Supplier { get; set; }
    }
}
