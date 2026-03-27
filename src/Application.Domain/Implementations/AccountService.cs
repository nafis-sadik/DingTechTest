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
                //throw new ArgumentException("Deposit amount cannot be less than or equal to zero");
                Console.WriteLine("Deposit amount cannot be less than or equal to zero");
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
            }
        }

        public async void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                //throw new ArgumentException("Withdraw amount cannot be less than or equal to zero");
                Console.WriteLine("Withdraw amount cannot be less than or equal to zero");
                return;
            }

            if (AccountEntity.CurrentBalance < amount)
            {
                //throw new ArgumentException("Insufficient balance for withdrawal.");
                Console.WriteLine("Insufficient balance for withdrawal.");
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
            }
        }

        public async void PrintStatement()
        {
            Console.WriteLine($"\n--- Fetching transactions for Account ID: {AccountEntity.AccountId} ---\n");
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                var transactions = await _transactionRepo.UnTrackableQuery()
                    .Where(t => t.To == AccountEntity.AccountId || t.From == AccountEntity.AccountId)
                    .OrderByDescending(t => t.TransactionTime)
                    .ToListAsync();

                if (transactions == null || !transactions.Any())
                {
                    Console.WriteLine("No transactions found for this account.");
                    return;
                }

                var statementLines = new List<string>();
                foreach (var tx in transactions)
                {
                    statementLines.Add($"{tx.TransactionTime:dd/MM/yyyy} || {tx.Amount,10:F2} || {tx.Balance,10:F2}");
                }

                Console.WriteLine("Date       || Amount     || Balance");
                statementLines.Reverse();
                foreach (var line in statementLines)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine($"\nCurrent Balance: {transactions.First().Balance:F2}");
                Console.WriteLine("\n--- End of Statement ---\n");
                Console.WriteLine("Press any key to return to the main menu...");
            }
        }
    }
}
