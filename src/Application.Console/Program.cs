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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("====================================================");
                Console.WriteLine("    WELCOME TO THE BANK ACCOUNT MANAGEMENT SYSTEM   ");
                Console.WriteLine("====================================================");
                Console.ResetColor();
                Console.WriteLine($"Currently Managing Account: {accountId}");
                Console.WriteLine("--- Customer: " + customer.CustomerName + " ---");
                Console.WriteLine();
                Console.WriteLine("Please select an option:");
                Console.WriteLine("  1. Deposit Funds");
                Console.WriteLine("  2. Withdraw Funds");
                Console.WriteLine("  3. Print Statement");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Tip: Type 'stop', 'exit', or 'x' to quit.");
                Console.ResetColor();
                Console.Write("\nChoice > ");
                
                userInputStr = Console.ReadLine();
                Console.Clear();                
                try
                {
                    if (int.TryParse(userInputStr, out int userInput))
                    {
                        switch (userInput)
                        {
                            case 1:
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(">>> DEPOSITING FUNDS <<<");
                                Console.ResetColor();
                                Console.Write("Enter the amount to deposit: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                                {
                                    accountService.Deposit(depositAmount);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error: Invalid amount. Please enter a valid decimal number.");
                                    Console.ResetColor();
                                }
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine(">>> WITHDRAWING FUNDS <<<");
                                Console.ResetColor();
                                Console.Write("Enter the amount to withdraw: ");
                                if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                                {
                                    accountService.Withdraw(withdrawAmount);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error: Invalid amount. Please enter a valid decimal number.");
                                    Console.ResetColor();
                                }
                                break;
                            case 3:
                                accountService.PrintStatement();
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Invalid option. Please select 1, 2, or 3.");
                                Console.ResetColor();
                                break;
                        }
                    }
 
                    if (string.IsNullOrEmpty(userInputStr) || loopBreakStatements.Contains(userInputStr?.ToLower()))
                        return;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Critical Error: {ex.Message}");
                    Console.ResetColor();
                }
 
                Console.WriteLine("\n[Press any key to return to the main menu]");
            } while (!loopBreakStatements.Contains(Console.ReadLine()?.ToLower()));
        }
    }
}
