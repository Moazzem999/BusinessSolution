using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("SupplierOrderDetails")]
    public class SupplierOrderDetailsEntity : BaseEntity
    {
        public long SupplierOrderId { get; set; }
        public long ProductId { get; set; }

        [MaxLength(100)]
        public string Remarks { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }



        [ForeignKey(nameof(SupplierOrderId))]
        public SupplierOrderEntity? SupplierOrder { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductEntity? Product { get; set; }
    }
}
