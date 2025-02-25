using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Entities;

namespace OrderService.Repositories;

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int orderId);
    Task<Order> AddAsync(Order newOrder);
    Task<Order> UpdateAsync(Order existingOrder);
    Task<bool> DeleteAsync(int orderId);
}
public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order> AddAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteAsync(int orderId)
    {
        var order = await GetByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}