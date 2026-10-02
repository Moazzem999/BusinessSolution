using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Entities.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
    }
}
