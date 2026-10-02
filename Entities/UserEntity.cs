using BusinessSolution.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSolution.Entities
{
    [Table("Users")]
    public class UserEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
