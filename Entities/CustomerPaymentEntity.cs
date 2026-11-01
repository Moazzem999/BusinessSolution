using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("CustomerPayments")]
    public class CustomerPaymentEntity : BaseEntity
    {
        public long CustomerId { get; set; }
        public string Remarks { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Payment { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public CustomerEntity? Customer { get; set; }
    }
}
