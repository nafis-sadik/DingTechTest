using Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DingTechTest.Configurations
{
    public static class DatabaseInitializer
    {
        public static async Task InitApplicationDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var context = provider.GetRequiredService<ApplicationDbContext>();
                var env = provider.GetRequiredService<IHostEnvironment>();

                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await context.Database.MigrateAsync();
                }

                if (env.IsDevelopment())
                {
                    await SeedData(context);
                }
            }
        }

        private static Task SeedData(ApplicationDbContext context)
        {
            // Seed data is not implemented yet. Keep as a no-op to avoid runtime errors during development.
            return Task.CompletedTask;
        }
    }
}
