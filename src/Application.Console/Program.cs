using Application.Domain.Implementations;
using Application.Domain.Models;
using DingTechTest.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

//namespace Program
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {

//        }
//    }
//}

// Use settings to ensure ContentRootPath points to the directory where appsettings.json is copied.
var options = new HostApplicationBuilderSettings { Args = args, ContentRootPath = AppDomain.CurrentDomain.BaseDirectory };
var builder = Host.CreateApplicationBuilder(options);

// Add services to the container.
builder.Services.RosolveDependencies(builder.Configuration);

await builder.Services.InitApplicationDatabase();

try
{
    var provider = builder.Services.BuildServiceProvider();
    var accountId = 1;
    var accountService = ActivatorUtilities.CreateInstance<AccountService>(provider, accountId);

    // 1. Deposit 1000 on 10-01-2012
    accountService.Deposit(new SingleTransactionRecord { AccountId = accountId, TransactionDate = new DateTime(2012, 1, 10), Amount = 1000 });

    // 2. Deposit 2000 on 13-01-2012
    accountService.Deposit(new SingleTransactionRecord { AccountId = accountId, TransactionDate = new DateTime(2012, 1, 13), Amount = 2000 });

    // 3. Withdraw 500 on 14-01-2012
    accountService.Withdraw(new SingleTransactionRecord { AccountId = accountId, TransactionDate = new DateTime(2012, 1, 14), Amount = 500 });

    // Output Statement
    accountService.PrintStatement();

    Console.WriteLine("------------------------------------------");

    // Test Case: Overdraft Scenario (Withdrawal exceeding current balance)
    // Current balance should be 2500 here. Attempting 5000.
    Console.WriteLine("\nTesting Overdraft Scenario (Attempting 5000 withdrawal)...");
    try
    {
        accountService.Withdraw(new SingleTransactionRecord
        {
            AccountId = accountId,
            TransactionDate = new DateTime(2012, 1, 15),
            Amount = 5000
        });
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

Console.WriteLine("------------------------------------------");
Console.WriteLine("Simulation Complete.");
Console.WriteLine("------------------------------------------");
