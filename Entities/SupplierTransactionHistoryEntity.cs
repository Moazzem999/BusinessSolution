using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("SupplierTransactionHistory")]
    public class SupplierTransactionHistoryEntity : BaseEntity
    {
        public long SupplierId { get; set; }
        public long? SupplierOrderId { get; set; }

        [MaxLength(50)]
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SupplierPreviousBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SupplierNewBalance { get; set; }

        public long? SupplierPaymentId { get; set; }


        [ForeignKey(nameof(SupplierId))]
        public SupplierEntity? Supplier { get; set; }

        [ForeignKey(nameof(SupplierOrderId))]
        public SupplierOrderEntity? SupplierOrder { get; set; }

        [ForeignKey(nameof(SupplierPaymentId))]
        public SupplierPaymentEntity? SupplierPayment { get; set; } 
    }
}
