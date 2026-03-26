using Application.Domain.Models;

namespace Application.Domain.Abstractions
{
    public interface ICustomerService
    {
        public Task<CustomerModel> AddCustomerAsync(CustomerModel customer);
        public Task UpdateCustomerAsync(CustomerModel customer);
        public Task DeleteCustomerAsync(string customerId);
        public Task<CustomerModel> GetCustomerById(string customerId);
        public Task<IList<CustomerModel>> GetAllAccountHolders();
    }
}
