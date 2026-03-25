using Application.Domain.Abstractions;
using Application.Domain.Models;
using Application.Entities;
using Ding.Core.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Domain.Implementations
{
    public class AccountService(IServiceProvider provider, int accountId) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = provider.GetRequiredService<ILogger<AccountService>>();
        private readonly IUnitOfWorkManager _unitOfWorkManager = provider.GetRequiredService<IUnitOfWorkManager>();

        public void Deposit(SingleTransactionRecord record)
        {
            if (record.Amount <= 0)
                throw new ArgumentException("Deposit amount cannot be less than or equal to zero");

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                Account? account = _accountRepo.TrackableQuery()
                    .FirstOrDefault(acc => acc.AccountId == record.AccountId);

                if (account == null)
                    throw new ArgumentException("Account not found");

                account.CurrentBalance += record.Amount;
                account.UpdatedAt = DateTime.UtcNow;
                _accountRepo.Update(account);

                _transactionRepo.InsertAsync(new Transaction
                {
                    From = null,
                    To = record.AccountId.ToString(),
                    Amount = record.Amount,
                    Time = DateTime.SpecifyKind(record.TransactionDate, DateTimeKind.Utc)
                }).Wait(); // Synchronous wait within void method

                _repositoryFactory.SaveChanges();
                _logger.LogInformation("Successfully deposited {Amount} to Account {AccountId}", record.Amount, record.AccountId);
            }
        }

        public void Withdraw(SingleTransactionRecord record)
        {
            if (record.Amount <= 0)
                throw new ArgumentException("Withdraw amount cannot be less than or equal to zero");

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                Account? account = _accountRepo.TrackableQuery()
                    .FirstOrDefault(acc => acc.AccountId == record.AccountId);

                if (account == null)
                    throw new ArgumentException("Account not found");

                if (account.CurrentBalance < record.Amount)
                {
                    throw new InvalidOperationException("Error: Insufficient balance for withdrawal.");
                }

                account.CurrentBalance -= record.Amount;
                account.UpdatedAt = DateTime.UtcNow;
                _accountRepo.Update(account);

                _transactionRepo.InsertAsync(new Transaction
                {
                    From = record.AccountId.ToString(),
                    To = null,
                    Amount = -record.Amount,
                    Time = DateTime.SpecifyKind(record.TransactionDate, DateTimeKind.Utc)
                }).Wait(); // Synchronous wait within void method

                _repositoryFactory.SaveChanges();
                _logger.LogInformation("Successfully withdrew {Amount} from Account {AccountId}", record.Amount, record.AccountId);
            }
        }

        public void PrintStatement()
        {
            Console.WriteLine($"\n--- Fetching transactions for Account ID: {accountId} ---\n");
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                var transactions = _transactionRepo.UnTrackableQuery()
                    .Where(t => t.To == accountId.ToString() || t.From == accountId.ToString())
                    .OrderBy(t => t.Time)
                    .ToList();

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
                    statementLines.Add($"{tx.Time:dd/MM/yyyy} || {tx.Amount,10:F2} || {currentBalance,10:F2}");
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
