using Ding.Core.Repositories;
using Ding.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ding.Core.EntityFramework
{
    public class EFRepositoryFactory : IRepositoryFactory, IDisposable
    {
        public readonly DbContext _dbContext;

        public EFRepositoryFactory(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DetachAllEntities()
        {
            var entries = _dbContext.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                entry.State = EntityState.Detached;
            }
        }

        public IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class => new RepositoryBase<TEntity>(_dbContext);

        public void SaveChanges() => _dbContext.SaveChanges();

        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();

        public async Task RollbackAsync() => await _dbContext.Database.RollbackTransactionAsync();

        public void Dispose()
        {
            // Do not dispose the dbContext here.
            // Since it is resolved from DI (scoped), the container should handle its lifecycle.
            // Disposing it here would prevent subsequent use of the context in the same scope.
            DetachAllEntities();
        }
    }
}
