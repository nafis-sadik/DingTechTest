using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Core.EntityFramework;
using System.Core.UnitOfWork;

namespace System.Core
{
    public static class CoreDependencyResolver<TDbContext> where TDbContext : DbContext
    {
        public static void RosolveCoreDependencies(IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder> optionsAction)
        {
            services.AddDbContext<TDbContext>(optionsAction);

            // Unit Of Work
            services.AddScoped<IRepositoryFactory, EFRepositoryFactory>();
            services.AddScoped<IUnitOfWorkManager, EFUnitOfWorkManager>();
        }
    }
}
