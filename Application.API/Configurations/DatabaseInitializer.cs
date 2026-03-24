using Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace DingTechTest.Configurations
{
    public static class DatabaseInitializer
    {
        public static async Task InitApplicationDatabase(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

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
