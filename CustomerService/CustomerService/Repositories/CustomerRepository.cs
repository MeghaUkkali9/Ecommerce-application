using CustomerService.Data;
using CustomerService.Data.Entities;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetCustomer(int id);
        Customer CreateCustomer(Customer customer);
        Customer UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DbContext _dbContext;

        public CustomerRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Customer GetCustomer(int id)
        {
            return _dbContext.Set<Customer>().FirstOrDefault(x => x.CustomerId == id);
        }

        public Customer CreateCustomer(Customer customer)
        {
            _dbContext.Set<Customer>().Add(customer);
            _dbContext.SaveChanges();
            return customer;
        }

        public Customer UpdateCustomer(Customer customer)
        {
            _dbContext.Set<Customer>().Update(customer);
            _dbContext.SaveChanges();
            return customer;
        }

        public void DeleteCustomer(Customer customer)
        {
            _dbContext.Set<Customer>().Remove(customer);
            _dbContext.SaveChanges();
        }
    }
}