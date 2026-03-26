namespace Application.Domain.Abstractions
{
    public interface IAccountService
    {
        public void Deposit(decimal amount);
        public void Withdraw(decimal amount);
        public void PrintStatement();
    }
}
