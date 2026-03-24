using Application.Entities;
using Microsoft.EntityFrameworkCore;
using System.Core;
using System.Core.Security;

namespace DingTechTest.Configurations
{
    public static class DependencyResolver
    {
        public static void RosolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // DB Context & Other relevant mappings for Blume Core Library using ApplicationDbContext (Database Agnostic)
            // This also registers ApplicationDbContext and maps the base DbContext to it.
            CoreDependencyResolver<ApplicationDbContext>.RosolveCoreDependencies(services, configuration, options =>
                options.UseNpgsql(configuration.GetConnectionString("ApplicationConnection")));

            // Services
            services.AddScoped<IClaimsPrincipalAccessor, HttpContextClaimsPrincipalAccessor>();
            //services.AddScoped<IInvoiceService, InvoiceService>();
        }
    }
}
