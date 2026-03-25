using Application.Domain.Abstractions;
using Application.Domain.Implementations;
using Application.Entities;
using Ding.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICustomerService, CustomerService>();
        }
    }
}
