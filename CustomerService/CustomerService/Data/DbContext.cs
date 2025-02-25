using CustomerService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<PaymentDetail> PaymentDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Customer>().ToTable("Customer");
        
        modelBuilder.Entity<PaymentDetail>().HasKey(c => c.PaymentId);
    }
}