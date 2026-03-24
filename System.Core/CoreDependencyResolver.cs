using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Core.EntityFramework;
using System.Core.Security;
using System.Core.UnitOfWork;

namespace System.Core
{
    public static class CoreDependencyResolver<TDbContext> where TDbContext : DbContext
    {
        public static void RosolveCoreDependencies(IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? optionsAction = null)
        {
            // Register as DbContext base class (required for System.Core generic repositories)
            if (optionsAction != null)
                services.AddDbContext<TDbContext>(optionsAction);
            else
                services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());

            // Unit Of Work
            services.AddScoped<IRepositoryFactory, EFRepositoryFactory>();
            services.AddScoped<IUnitOfWorkManager, EFUnitOfWorkManager>();

            // Claims
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IClaimsPrincipalAccessor, HttpContextClaimsPrincipalAccessor>();
        }
    }
}
