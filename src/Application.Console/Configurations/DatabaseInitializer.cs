using Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DingTechTest.Configurations
{
    public static class DatabaseInitializer
    {
        public static async Task InitApplicationDatabase(this IServiceCollection services)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var provider = scope.ServiceProvider;
                var context = provider.GetRequiredService<ApplicationDbContext>();

                // Ensure the database and schema are created.
                if ((await context.Database.GetPendingMigrationsAsync()).Any())
                {
                    await context.Database.MigrateAsync();
                }

                await SeedData(context);
            }
        }

        private async static Task SeedData(ApplicationDbContext context)
        {
            // Seed a default customer if not present
            if (!await context.Set<Customer>().AnyAsync())
            {
                string defaultCustomerId = Guid.NewGuid().ToString();
                await context.Set<Customer>().AddAsync(new Customer
                {
                    CustomerId = defaultCustomerId,
                    CustomerName = "John Doe",
                    Email = "john@example.com",
                    PhoneNumber = "1234567890",
                });

                // Seed a default account for this customer
                await context.Set<Account>().AddAsync(new Account
                {
                    AccountHolderId = defaultCustomerId,
                    AccountTitle = "Main Savings",
                    CurrentBalance = 0,
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
