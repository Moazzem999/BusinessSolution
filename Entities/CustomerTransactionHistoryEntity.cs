using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("CustomerTransactionHistory")]
    public class CustomerTransactionHistoryEntity : BaseEntity
    {
        public long CustomerId { get; set; }
        public long? CustomerOrderId { get; set; }

        [MaxLength(50)]
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomerPreviousBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomerNewBalance { get; set; }

        public long? CustomerPaymentId { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public CustomerEntity? Customer { get; set; }

        [ForeignKey(nameof(CustomerOrderId))]
        public CustomerOrderEntity? CustomerOrder { get; set; }

        [ForeignKey(nameof(CustomerPaymentId))]
        public CustomerPaymentEntity? CustomerPayment { get; set; }
    }
}
