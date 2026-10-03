using BusinessSolution.Entities.Common;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BusinessSolution.Entities.Context
{
    public abstract class BaseDbContext(DbContextOptions options, ICurrentUserService currentUserService) : DbContext(options)
    {
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = currentUserService.UserId;

            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedOn = DateTimeOffset.Now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Status = Status.Active;

                    entry.Entity.UpdatedOn = DateTimeOffset.Now;
                    entry.Entity.UpdatedBy = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedOn = DateTimeOffset.Now;
                    entry.Entity.UpdatedBy = userId;

                    // Prevent overwriting Created fields
                    entry.Property(x => x.CreatedOn).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
