using Application.Domain.Models;

namespace Application.Domain.Abstractions
{
    public interface IAccountService
    {
        public void Deposit(TransactionRecord record);
        public void Withdraw(TransactionRecord record);
        public void PrintStatement();
    }
}
