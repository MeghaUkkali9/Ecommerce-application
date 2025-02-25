using Microsoft.EntityFrameworkCore;
using PaymentService.Data;

namespace PaymentService.Repositories;

public interface IPaymentRepository
{
    Task<Payment> AddPaymentAsync(Payment payment);
    Task<Payment> GetPaymentByOrderIdAsync(int orderId);
}

public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentDbContext _context;

    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> AddPaymentAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }
}