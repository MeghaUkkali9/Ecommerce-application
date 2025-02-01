using CustomerService.Data.Entities;
using CustomerService.DTOs;

namespace CustomerService.Mappers
{
    public interface ICustomerMapper
    {
        Customer MapToEntity(CreateCustomerDto createCustomerDto);
        void UpdateEntityFromDto(Customer customer, UpdateCustomerDto updateCustomerDto);
        CustomerDto MapToDto(Customer customer);
    }

    public class CustomerMapper : ICustomerMapper
    {
        public Customer MapToEntity(CreateCustomerDto createCustomerDto)
        {
            return new Customer
            {
                FirstName = createCustomerDto.FirstName,
                LastName = createCustomerDto.LastName,
                Email = createCustomerDto.Email,
                Phone = createCustomerDto.Phone
            };
        }

        public void UpdateEntityFromDto(Customer customer, UpdateCustomerDto updateCustomerDto)
        {
            customer.FirstName = updateCustomerDto.FirstName;
            customer.LastName = updateCustomerDto.LastName;
            customer.Email = updateCustomerDto.Email;
            customer.Phone = updateCustomerDto.Phone;
        }

        public CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone
            };
        }
    }
}