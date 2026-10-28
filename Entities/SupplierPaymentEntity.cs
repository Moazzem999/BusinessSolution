using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("SupplierPayments")]
    public class SupplierPaymentEntity : BaseEntity
    {
        public long SupplierId { get; set; }
        public string Remarks { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }


        [ForeignKey(nameof(SupplierId))]
        public SupplierEntity? Supplier { get; set; }
    }
}
