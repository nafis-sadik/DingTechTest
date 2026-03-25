using Ding.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ding.Core.EntityFramework
{
    public class EFUnitOfWorkManager(DbContext dbContext) : IUnitOfWorkManager
    {
        private readonly DbContext _dbContext = dbContext;

        public IRepositoryFactory GetRepositoryFactory() => new EFRepositoryFactory(_dbContext);
        public IDbContextTransaction BeginTransaction() => _dbContext.Database.BeginTransaction();
        public async Task CommitAsync() => await _dbContext.Database.CommitTransactionAsync();
        public async Task RollbackAsync() => await _dbContext.Database.RollbackTransactionAsync();
    }
}
