using Application.Domain.Abstractions;
using Application.Domain.Models;
using Application.Entities;
using Ding.Core.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Entity;
using System.Security.Principal;

namespace Application.Domain.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly Account AccountEnttiy;

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

                if(account == null)
                    throw new ArgumentException("Account not found");

                AccountEnttiy = account;
            }
        }

        public async void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount cannot be less than or equal to zero");

            AccountEnttiy.CurrentBalance += amount;
            AccountEnttiy.UpdatedAt = DateTime.UtcNow;

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                _accountRepo.ColumnUpdate(
                    AccountEnttiy.AccountId,
                    new Dictionary<string, object>
                    {
                        { nameof(AccountEnttiy.CurrentBalance), AccountEnttiy.CurrentBalance },
                        { nameof(AccountEnttiy.UpdatedAt), AccountEnttiy.UpdatedAt }
                    }
                );

                _transactionRepo.InsertAsync(new Transaction
                {
                    From = null,
                    To = AccountEnttiy.AccountId,
                    Amount = amount,
                    TransactionTime = DateTime.UtcNow,
                }).Wait(); // Synchronous wait within void method

                _repositoryFactory.SaveChanges();
                _logger.LogInformation("Successfully deposited {Amount} to Account {AccountId}", amount, AccountEnttiy.AccountId);
            }
        }

        public async void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdraw amount cannot be less than or equal to zero");

            if (AccountEnttiy.CurrentBalance < amount)
                throw new InvalidOperationException("Error: Insufficient balance for withdrawal.");

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                AccountEnttiy.CurrentBalance -= amount;
                AccountEnttiy.UpdatedAt = DateTime.UtcNow;
                _accountRepo.ColumnUpdate(
                    AccountEnttiy.AccountId,
                    new Dictionary<string, object>
                    {
                        { nameof(AccountEnttiy.CurrentBalance), AccountEnttiy.CurrentBalance },
                        { nameof(AccountEnttiy.UpdatedAt), AccountEnttiy.UpdatedAt }
                    }
                );

                await _transactionRepo.InsertAsync(new Transaction
                {
                    From = AccountEnttiy.AccountId,
                    To = null,
                    Amount = amount,
                    TransactionTime = DateTime.UtcNow,
                }); // Synchronous wait within void method

                _repositoryFactory.SaveChanges();
                _logger.LogInformation("Successfully withdrew {Amount} from Account {AccountId}", amount, AccountEnttiy.AccountId);
            }
        }

        public async void PrintStatement()
        {
            Console.WriteLine($"\n--- Fetching transactions for Account ID: {AccountEnttiy.AccountId} ---\n");
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                var transactions = await _transactionRepo.UnTrackableQuery()
                    .Where(t => t.To == AccountEnttiy.AccountId || t.From == AccountEnttiy.AccountId)
                    .OrderByDescending(t => t.TransactionTime)
                    .ToListAsync();

                if (transactions == null || !transactions.Any())
                {
                    Console.WriteLine("No transactions found for this account.");
                    return;
                }

                decimal currentBalance = 0;
                var statementLines = new List<string>();

                foreach (var tx in transactions)
                {
                    currentBalance += tx.Amount;
                    statementLines.Add($"{tx.TransactionTime:dd/MM/yyyy} || {tx.Amount,10:F2} || {currentBalance,10:F2}");
                }

                Console.WriteLine("Date       || Amount     || Balance");
                statementLines.Reverse();
                foreach (var line in statementLines)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
