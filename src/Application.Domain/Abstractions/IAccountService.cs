using Application.Domain.Models;

namespace Application.Domain.Abstractions
{
    public interface IAccountService
    {
        public void Deposit(SingleTransactionRecord record);
        public void Withdraw(SingleTransactionRecord record);
        public void PrintStatement();
    }
}
