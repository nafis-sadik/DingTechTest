using Application.Domain.Abstractions;
using Application.Domain.Models;
using Application.Entities;
using Microsoft.Extensions.Logging;
using System.Core.UnitOfWork;
using System.Data.Entity;

namespace Application.Domain.Implementations
{
    public class AccountService(IUnitOfWorkManager unitOfWorkManager, ILogger<AccountService> logger) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

        public async void Deposit(SingleTransactionRecord record)
        {
            if (record.Amount <= 0)
                throw new ArgumentException("Diposit amount can not be less than or equal to zero");

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                // Validation
                Account? account = await _accountRepo.UnTrackableQuery()
                    .Where(account => account.AccountHolderId == record.AccountHolderId && account.AccountId == record.AccountId)
                    .Select(account => new Account
                    {
                        AccountId = account.AccountId,
                        CurrentBalance = account.CurrentBalance,
                    })
                    .FirstOrDefaultAsync();

                if (account == null)
                    throw new ArgumentException("Account not found");

                // Deposit
                _accountRepo.ColumnUpdate(account.AccountId, new Dictionary<string, object> {
                    { nameof(account.CurrentBalance), account.CurrentBalance + record.Amount },
                });

                // Transaction history
                await _transactionRepo.InsertAsync(new Transaction
                {
                    From = null,
                    To = record.AccountHolderId,
                    Amount = record.Amount,
                });

                await _repositoryFactory.SaveChangesAsync();
            }
        }

        public async void Transfer(AccountToAccountTransactionRecord record)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();

                Account? senderAcc = await _accountRepo.UnTrackableQuery()
                    .FirstOrDefaultAsync(account => account.AccountId == record.SenderAccountNo);

                Account? recieverAcc = await _accountRepo.UnTrackableQuery()
                    .FirstOrDefaultAsync(account => account.AccountId == record.RecieverAccountNo && account.AccountHolderId == record.RecieverAccountHolderId);

                if (senderAcc == null)
                    throw new ArgumentException("Sender account not found");

                if (recieverAcc == null)
                    throw new ArgumentException("Reciever account not found");

                if (recieverAcc.CurrentBalance < record.Amount)
                    throw new ArgumentException("Insufficient Balance");

                // Send money
                _accountRepo.ColumnUpdate(senderAcc.AccountId,
                    new Dictionary<string, object> {
                        { nameof(senderAcc.CurrentBalance), senderAcc.CurrentBalance - record.Amount }
                    });

                // Recieve money
                _accountRepo.ColumnUpdate(recieverAcc.AccountId,
                    new Dictionary<string, object> {
                        { nameof(recieverAcc.CurrentBalance), recieverAcc.CurrentBalance + record.Amount }
                    });

                await _repositoryFactory.SaveChangesAsync();
            }
        }

        public void PrintStatement()
        {
            throw new NotImplementedException();
        }

        public async void Withdraw(SingleTransactionRecord record)
        {
            if (record.Amount <= 0)
                throw new ArgumentException("Withdraw amount can not be less than or equal to zero");

            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _accountRepo = _repositoryFactory.GetRepository<Account>();
                var _transactionRepo = _repositoryFactory.GetRepository<Transaction>();

                // Validation
                Account? account = await _accountRepo.UnTrackableQuery()
                    .Where(account => account.AccountHolderId == record.AccountHolderId && account.AccountId == record.AccountId)
                    .Select(account => new Account
                    {
                        AccountId = account.AccountId,
                        CurrentBalance = account.CurrentBalance,
                    })
                    .FirstOrDefaultAsync();

                if (account == null)
                    throw new ArgumentException("Account not found");

                // Withdraw
                _accountRepo.ColumnUpdate(account.AccountId, new Dictionary<string, object> {
                    { nameof(account.CurrentBalance), account.CurrentBalance - record.Amount },
                });

                // Transaction history
                await _transactionRepo.InsertAsync(new Transaction
                {
                    From = null,
                    To = record.AccountHolderId,
                    Amount = record.Amount,
                });

                await _repositoryFactory.SaveChangesAsync();
            }
        }
    }
}
