using Application.Domain.Abstractions;
using Application.Entities;
using Ding.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Domain.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly Account AccountEntity;

        public AccountService(IServiceProvider provider, int accountId)
        {
            _logger = provider.GetRequiredService<ILogger<AccountService>>();
            _unitOfWorkManager = provider.GetRequiredService<IUnitOfWorkManager>();
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();

                Account? account = _accountRepo.UnTrackableQuery()
                    .Where(account => account.AccountId == accountId)
                    .FirstOrDefault();

                if (account == null)
                {
                    //throw new ArgumentException("Account not found");
                    Console.WriteLine("Account not found");
                    return;
                }

                AccountEntity = account;
            }
        }

        public async void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Deposit amount must be greater than zero.");
                Console.ResetColor();
                return;
            }
 
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();
 
                AccountEntity.CurrentBalance += amount;
                AccountEntity.UpdatedAt = DateTime.UtcNow;
                _accountRepo.ColumnUpdate(
                    AccountEntity.AccountId,
                    new Dictionary<string, object>
                    {
                        { nameof(AccountEntity.CurrentBalance), AccountEntity.CurrentBalance },
                        { nameof(AccountEntity.UpdatedAt), AccountEntity.UpdatedAt }
                    }
                );
 
                await _transactionRepo.InsertAsync(new Transaction
                {
                    From = null,
                    To = AccountEntity.AccountId,
                    Amount = amount,
                    Balance = AccountEntity.CurrentBalance,
                    TransactionTime = DateTime.UtcNow,
                }); // Synchronous wait within void method
 
                await _repositoryFactory.SaveChangesAsync();
                _logger.LogInformation($"Successfully deposited {amount} to Account {AccountEntity.AccountId}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Success: Deposited {amount:F2}. New Balance: {AccountEntity.CurrentBalance:F2}");
                Console.ResetColor();
            }
        }

        public async void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Withdraw amount must be greater than zero.");
                Console.ResetColor();
                return;
            }
 
            if (AccountEntity.CurrentBalance < amount)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Insufficient balance for withdrawal.");
                Console.ResetColor();
                return;
            }

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                AccountEntity.CurrentBalance -= amount;
                AccountEntity.UpdatedAt = DateTime.UtcNow;
                _accountRepo.ColumnUpdate(
                    AccountEntity.AccountId,
                    new Dictionary<string, object>
                    {
                        { nameof(AccountEntity.CurrentBalance), AccountEntity.CurrentBalance },
                        { nameof(AccountEntity.UpdatedAt), AccountEntity.UpdatedAt }
                    }
                );

                await _transactionRepo.InsertAsync(new Transaction
                {
                    From = AccountEntity.AccountId,
                    To = null,
                    Amount = amount,
                    Balance = AccountEntity.CurrentBalance,
                    TransactionTime = DateTime.UtcNow,
                }); // Synchronous wait within void method

                await _repositoryFactory.SaveChangesAsync();
                _logger.LogInformation($"Successfully withdrew {amount} from Account {AccountEntity.AccountId}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Success: Withdrew {amount:F2}. New Balance: {AccountEntity.CurrentBalance:F2}");
                Console.ResetColor();
            }
        }

        public async void PrintStatement()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n--- ACCOUNT STATEMENT (ID: {AccountEntity.AccountId}) ---");
            Console.ResetColor();
            
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();
 
                var transactions = await _transactionRepo.UnTrackableQuery()
                    .Where(t => t.To == AccountEntity.AccountId || t.From == AccountEntity.AccountId)
                    .OrderByDescending(t => t.TransactionTime)
                    .ToListAsync();
 
                if (transactions == null || !transactions.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("No transactions found for this account.");
                    Console.ResetColor();
                    return;
                }
 
                var statementLines = new List<string>();
                foreach (var tx in transactions)
                {
                    var bdTime = tx.TransactionTime.AddHours(6);
                    statementLines.Add($"{bdTime:dd/MM/yyyy} || {bdTime:HH:mm} || {tx.Amount,10:F2} || {tx.Balance,10:F2}");
                }
 
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("----------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Date       || Time  ||   Amount   ||   Balance  ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("----------------------------------------------------");
                Console.ResetColor();
                
                statementLines.Reverse();
                foreach (var line in statementLines)
                {
                    Console.WriteLine(line);
                }
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("----------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"CURRENT BALANCE: {transactions.First().Balance:F2}");
                Console.ResetColor();
                Console.WriteLine("--- End of Statement ---\n");
            }
        }
    }
}
