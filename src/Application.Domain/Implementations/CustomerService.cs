using Application.Domain.Abstractions;
using Ding.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using DomainModels = Application.Domain.Models;

namespace Application.Domain.Implementations
{
    public class CustomerService(IUnitOfWorkManager unitOfWorkManager, ILogger<CustomerService> logger) : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger = logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

        public async Task AddCustomerAsync(DomainModels.Customer customer)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();

                await _customerRepo.InsertAsync(new Entities.Customer
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                });

                await _repositoryFactory.SaveChangesAsync();
                _logger.LogInformation("Successfully added customer {CustomerName} ({CustomerId})", customer.CustomerName, customer.CustomerId);
            }
        }

        public async Task UpdateCustomerAsync(DomainModels.Customer customer)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();

                var existing = await _customerRepo.GetAsync(customer.CustomerId);
                if (existing == null) throw new ArgumentException("Customer not found.");

                existing.CustomerName = customer.CustomerName;
                existing.Email = customer.Email;
                existing.PhoneNumber = customer.PhoneNumber;

                _customerRepo.Update(existing);
                await _repositoryFactory.SaveChangesAsync();
                _logger.LogInformation("Successfully updated customer {CustomerId}", customer.CustomerId);
            }
        }

        public async Task DeleteCustomerAsync(string customerId)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();
                await _customerRepo.DeleteAsync(customerId);
                await _repositoryFactory.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted customer {CustomerId}", customerId);
            }
        }
    }
}
