using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("Products")]
    public class ProductEntity : BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Details { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ImagePath { get; set; }
    }
}
