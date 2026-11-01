using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("CustomerOrders")]
    public class CustomerOrderEntity : BaseEntity
    {
        public long CustomerId { get; set; }

        [MaxLength(50)]
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomerPreviousBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CustomerNewBalance { get; set; }


        [ForeignKey(nameof(CustomerId))]
        public CustomerEntity? Customer { get; set; }

        public ICollection<CustomerOrderDetailsEntity> OrderDetails { get; set; } = new List<CustomerOrderDetailsEntity>();
    }
}
