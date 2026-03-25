using Application.Domain.Models;

namespace Application.Domain.Abstractions
{
    public interface ICustomerService
    {
        public Task AddCustomerAsync(Customer customer);
        public Task UpdateCustomerAsync(Customer customer);
        public Task DeleteCustomerAsync(string customerId);
    }
}
