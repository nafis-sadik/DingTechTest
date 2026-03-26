using Application.Domain.Abstractions;
using Application.Domain.Implementations;
using Application.Domain.Models;
using DingTechTest.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Program
{
    public class Program
    {
        private static async Task<ServiceProvider> ProjectInitialization(string[] args)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.RosolveDependencies(builder.Configuration);
            await builder.Services.InitApplicationDatabase();
            return builder.Services.BuildServiceProvider();
        }

        public static async Task Main(string[] args)
        {
            try
            {
                var serviceProvider = await ProjectInitialization(args);
                var customerService = serviceProvider.GetService<ICustomerService>();
                if (customerService == null)
                    throw new ArgumentException("CustomerService not found in the service provider. Please check the dependency injection configuration.");

                var customers = await customerService.GetAllCustomers();
                if (customers == null || !customers.Any())
                    throw new ArgumentException("No customers found in the database. Please seed the database with appropriate data before running the application.");

                var customer = customers.FirstOrDefault(customer => customer.AccountNumberList.Count() > 0);
                if (customer == null)
                    throw new ArgumentException("No customers with accounts found in the database. Please seed the database with appropriate data before running the application.");

                int accountId = customer.AccountNumberList.First();
                IAccountService accountService = new AccountService(serviceProvider, accountId);

                // 1. Deposit 1000 on 10-01-2012
                accountService.Deposit(1000);

                // 2. Deposit 2000 on 13-01-2012
                accountService.Deposit(2000);

                // 3. Withdraw 500 on 14-01-2012
                accountService.Withdraw(500);

                // Output Statement
                accountService.PrintStatement();

                // Test Case: Overdraft Scenario (Withdrawal exceeding current balance)
                // Current balance should be 2500 here. Attempting 5000.
                Console.WriteLine("\nTesting Overdraft Scenario (Attempting 5000 withdrawal)...");
                try
                {
                    accountService.Withdraw(5000);
                    Console.WriteLine("FAILED: Withdraw should have thrown balance error!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SUCCESS: Caught Expected Error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
