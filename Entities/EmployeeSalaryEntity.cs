using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("EmployeeSalaries")]
    public class EmployeeSalaryEntity : BaseEntity
    {
        public long EmployeeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdvancePayment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BonusPayment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OthersBill { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [MaxLength(20)]
        public string PaySlipFor  { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Remarks { get; set; } = string.Empty;


        [ForeignKey(nameof(EmployeeId))]
        public EmployeeEntity? Employee { get; set; }
    }
}
