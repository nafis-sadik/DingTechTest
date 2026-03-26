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
        private static HashSet<string> loopBreakStatements = new HashSet<string>
        {
            "stop", "exit", "x"
        };

        private static async Task<ServiceProvider> ApplicationInitialization(string[] args)
        {
            try
            {
                var builder = Host.CreateApplicationBuilder();

                builder.Services.RosolveDependencies(builder.Configuration);
                await builder.Services.InitApplicationDatabase();
                return builder.Services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unexpected error: {ex.ToString()}");
            }
        }

        public static async Task Main(string[] args)
        {
            var serviceProvider = await ApplicationInitialization(args);
            var customerService = serviceProvider.GetService<ICustomerService>();
            if (customerService == null)
                throw new ArgumentException("CustomerService not found in the service provider. Please check the dependency injection configuration.");

            CustomerModel? customer = (await customerService.GetAllAccountHolders()).FirstOrDefault();
            if (customer == null)
                throw new ArgumentException("No customers with accounts found in the database. Please seed the database with appropriate data before running the application.");

            int accountId = customer.AccountNumberList.First();
            IAccountService accountService = new AccountService(serviceProvider, accountId);
            string? userInputStr = string.Empty;
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Bank Account Management System!");
                Console.WriteLine("Please enter a command (type 'stop', 'exit', or 'x' to quit):");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. PrintStatement");
                userInputStr = Console.ReadLine();
                if (int.TryParse(userInputStr, out int userInput))
                {
                    try
                    {
                        Console.Clear();
                        switch (userInput)
                        {
                            case 1:
                                Console.WriteLine("Enter the amount to deposit:");
                                if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                                {
                                    accountService.Deposit(depositAmount);
                                    Console.WriteLine($"Successfully deposited {depositAmount:C}.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
                                }
                                break;
                            case 2:
                                Console.WriteLine("Enter the amount to withdraw:");
                                if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                                {
                                    accountService.Withdraw(withdrawAmount);
                                    Console.WriteLine($"Successfully withdrew {withdrawAmount:C}.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
                                }
                                break;
                            case 3:
                                accountService.PrintStatement();
                                break;
                            default:
                                Console.WriteLine("Invalid option. Please select 1, 2, or 3.");
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing your request: {ex.Message}");
                    }
                }
            } while (!loopBreakStatements.Contains(Console.ReadLine()));
        }
    }
}
