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
        public DbSet<EmployeeSalaryEntity> EmployeeSalaries { get; set; }
        public DbSet<SupplierEntity> Suppliers { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<SupplierOrderEntity> SupplierOrders { get; set; }
        public DbSet<SupplierOrderDetailsEntity> SupplierOrderDetails { get; set; }
        public DbSet<SupplierTransactionHistoryEntity> SupplierTransactionHistories { get; set; }
        public DbSet<SupplierPaymentEntity> SupplierPayments { get; set; }
    }
}
