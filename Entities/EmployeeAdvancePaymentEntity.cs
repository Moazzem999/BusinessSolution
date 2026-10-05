using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("EmployeeAdvancePayments")]
    public class EmployeeAdvancePaymentEntity : BaseEntity
    {
        public long EmployeeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;


        [ForeignKey(nameof(EmployeeId))]
        public EmployeeEntity? Employee { get; set; }
    }
}
