using BusinessSolution.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessSolution.Entities.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options,
    ICurrentUserService currentUserService) : BaseDbContext(options, currentUserService)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<EmployeeAdvancePaymentEntity> EmployeeAdvancePayments { get; set; }
    }
}
