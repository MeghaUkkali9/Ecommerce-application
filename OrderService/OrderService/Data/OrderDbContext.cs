using Microsoft.EntityFrameworkCore;
using OrderService.Data.Entities;

namespace OrderService.Data;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
    }
}