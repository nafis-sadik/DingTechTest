using Application.Domain.Abstractions;
using Application.Domain.Models;

namespace Application.Domain.Implementations
{
    public class CustomerService : ICustomerService
    {
        public Task AddCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCustomerAsync(string customerId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
