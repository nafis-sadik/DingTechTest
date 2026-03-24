using Application.Domain.Models;

namespace Application.Domain.Abstractions
{
    public interface ICustomerService
    {
        public void AddCustomer(Customer customer);
        public void UpdateCustomer(Customer customer);
        public void DeleteCustomer(string customerId);
    }
}
