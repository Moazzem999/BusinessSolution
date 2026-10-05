using BusinessSolution.Shared.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities.Common
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public Status Status { get; set; } = Status.Active;
    }
}
