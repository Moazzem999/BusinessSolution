using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("Suppliers")]
    public class SupplierEntity : BaseEntity
    {
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string ShopName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string NidNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Mobile { get; set; } = string.Empty;

        [MaxLength(500)]
        public string AdditionalDetails { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PresentAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PermanentAddress { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; }
        
        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [MaxLength(255)]
        public string? NidImagePath { get; set; }

        [MaxLength(255)]
        public string? ChequeImagePath { get; set; }
    }
}
