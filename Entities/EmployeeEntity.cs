using BusinessSolution.Entities.Common;
using BusinessSolution.Shared.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("Employees")]
    public class EmployeeEntity : BaseEntity
    {
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string FatherName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NidNo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Mobile { get; set; } = string.Empty;

        public DateTimeOffset? DateOfBirth { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public Religion Religion { get; set; }

        [MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(50)]
        public string AcademicQualification { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PresentAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PermanentAddress { get; set; } = string.Empty;

        public DateTimeOffset? JoiningDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [MaxLength(255)]
        public string? NidImagePath { get; set; }
    }
}
