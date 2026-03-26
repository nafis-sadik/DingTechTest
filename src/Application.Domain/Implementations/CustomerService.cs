using Application.Domain.Abstractions;
using Application.Domain.Models;
using Ding.Core.UnitOfWork;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace Application.Domain.Implementations
{
    public class CustomerService(IUnitOfWorkManager unitOfWorkManager, ILogger<CustomerService> logger) : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger = logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager = unitOfWorkManager;

        public async Task<CustomerModel> AddCustomerAsync(CustomerModel customer)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();

                Entities.Customer newCustomer = new Entities.Customer
                {
                    CustomerId = Guid.NewGuid().ToString(),
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                };
                newCustomer = await _customerRepo.InsertAsync(newCustomer);
                await _repositoryFactory.SaveChangesAsync();

                _logger.LogInformation("Successfully added customer {CustomerName} ({CustomerId})", customer.CustomerName, customer.CustomerId);

                return new CustomerModel
                {
                    CustomerId = newCustomer.CustomerId,
                    CustomerName = newCustomer.CustomerName,
                    Email = newCustomer.Email,
                    PhoneNumber = newCustomer.PhoneNumber
                };
            }
        }

        public async Task UpdateCustomerAsync(CustomerModel customer)
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

        public async Task<CustomerModel> GetCustomerById(string customerId)
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();
                var customer = await _customerRepo.GetAsync(customerId);
                if (customer == null) throw new ArgumentException("Customer not found.");
                return new CustomerModel
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                };
            }
        }

        public async Task<IEnumerable<CustomerModel>> GetAllCustomers()
        {
            using (var _repositoryFactory = _unitOfWorkManager.GetRepositoryFactory())
            {
                var _customerRepo = _repositoryFactory.GetRepository<Entities.Customer>();
                var qyery = _customerRepo.UnTrackableQuery();
                qyery = qyery.Where(customer => customer.CustomerId.Length > 0);
                var qyery2 = qyery.Select(c => new Entities.Customer
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Accounts = c.Accounts.Select(account => new Entities.Account { AccountId = account.AccountId }).ToList()
                });

                var qyery2Result = await qyery2.ToListAsync();
                List<CustomerModel> customerModels = new List<CustomerModel>();
                foreach (var customer in qyery2Result)
                {
                    customerModels.Add(new CustomerModel
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.CustomerName,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        AccountNumberList = customer.Accounts.Select(a => a.AccountId).ToList()
                    });
                }

                return customerModels;
            }
        }
    }
}
