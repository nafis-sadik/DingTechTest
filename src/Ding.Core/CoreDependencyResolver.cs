using Ding.Core.EntityFramework;
using Ding.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ding.Core
{
    public static class CoreDependencyResolver<TDbContext> where TDbContext : DbContext
    {
        public static void RosolveCoreDependencies(IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddDbContext<TDbContext>(optionsAction);

            // Register as DbContext base class (required for EFUnitOfWorkManager)
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

            // Unit Of Work
            services.AddScoped<IRepositoryFactory, EFRepositoryFactory>();
            services.AddScoped<IUnitOfWorkManager, EFUnitOfWorkManager>();
        }
    }
}
