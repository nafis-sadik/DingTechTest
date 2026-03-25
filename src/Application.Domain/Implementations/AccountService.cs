using Application.Domain.Abstractions;
using Application.Domain.Models;

namespace Application.Domain.Implementations
{
    public class AccountService : IAccountService
    {
        public void Deposit(TransactionRecord record)
        {
            throw new NotImplementedException();
        }

        public void PrintStatement()
        {
            throw new NotImplementedException();
        }

        public void Withdraw(TransactionRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
