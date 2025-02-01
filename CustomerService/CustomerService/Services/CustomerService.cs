using CustomerService.DTOs;
using CustomerService.Mappers;
using CustomerService.Repositories;

namespace CustomerService.Services
{
    public interface ICustomerService
    {
        CustomerDto GetCustomerById(int id);
        CustomerDto CreateCustomer(CreateCustomerDto createCustomerDto);
        CustomerDto UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto);
        bool DeleteCustomer(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerMapper _customerMapper;

        public CustomerService(ICustomerRepository customerRepository, ICustomerMapper customerMapper)
        {
            _customerRepository = customerRepository;
            _customerMapper = customerMapper;
        }

        public CustomerDto GetCustomerById(int id)
        {
            var customer = _customerRepository.GetCustomer(id);
            return customer != null ? _customerMapper.MapToDto(customer) : null;
        }

        public CustomerDto CreateCustomer(CreateCustomerDto createCustomerDto)
        {
            var customerEntity = _customerMapper.MapToEntity(createCustomerDto);
            var createdCustomer = _customerRepository.CreateCustomer(customerEntity);
            return _customerMapper.MapToDto(createdCustomer);
        }

        public CustomerDto UpdateCustomer(int id, UpdateCustomerDto updateCustomerDto)
        {
            var existingCustomer = _customerRepository.GetCustomer(id);
            if (existingCustomer == null) return null;

            _customerMapper.UpdateEntityFromDto(existingCustomer, updateCustomerDto);
            var updatedCustomer = _customerRepository.UpdateCustomer(existingCustomer);
            return _customerMapper.MapToDto(updatedCustomer);
        }

        public bool DeleteCustomer(int id)
        {
            var customer = _customerRepository.GetCustomer(id);
            if (customer == null) return false;

            _customerRepository.DeleteCustomer(customer);
            return true;
        }
    }
}
