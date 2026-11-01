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
        public DbSet<ProductEntity> Products { get; set; }

        public DbSet<SupplierEntity> Suppliers { get; set; }    
        public DbSet<SupplierOrderEntity> SupplierOrders { get; set; }
        public DbSet<SupplierOrderDetailsEntity> SupplierOrderDetails { get; set; }
        public DbSet<SupplierPaymentEntity> SupplierPayments { get; set; }
        public DbSet<SupplierTransactionHistoryEntity> SupplierTransactionHistories { get; set; }
        
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<CustomerOrderEntity> CustomerOrders { get; set; }
        public DbSet<CustomerOrderDetailsEntity> CustomerOrderDetails { get; set; }
        public DbSet<CustomerPaymentEntity> CustomerPayments { get; set; }
        public DbSet<CustomerTransactionHistoryEntity> CustomerTransactionHistories { get; set; }
    }
}
