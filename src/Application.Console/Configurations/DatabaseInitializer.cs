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
                await context.Database.EnsureCreatedAsync();

                await SeedData(context);
            }
        }

        private async static Task SeedData(ApplicationDbContext context)
        {
            // Seed a default customer if not present
            if (!await context.Set<Customer>().AnyAsync())
            {
                var customer = new Customer
                {
                    CustomerId = "C001",
                    CustomerName = "John Doe",
                    Email = "john@example.com"
                };
                await context.Set<Customer>().AddAsync(customer);

                // Seed a default account for this customer
                var account = new Account
                {
                    AccountHolderId = "C001",
                    AccountTitle = "Main Savings",
                    CurrentBalance = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Set<Account>().AddAsync(account);

                await context.SaveChangesAsync();
            }
        }
    }
}
