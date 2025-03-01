using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Mappers;
using PaymentService.Repositories;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<PaymentDto> ProcessPaymentAsync(int orderId, decimal amount, PaymentMethod paymentMethod, PaymentStatus paymentStatus);
    Task<PaymentDto> GetPaymentStatusAsync(int orderId);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentMapper _paymentMapper;

    public PaymentService(IPaymentRepository paymentRepository, IPaymentMapper paymentMapper)
    {
        _paymentRepository = paymentRepository;
        _paymentMapper = paymentMapper;
    }

    public async Task<PaymentDto> ProcessPaymentAsync(int orderId,
        decimal amount, 
        PaymentMethod paymentMethod, PaymentStatus paymentStatus)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaymentStatus = paymentStatus,
            PaymentDate = DateTime.Now,
            PaymentMethod = paymentMethod
        };
        
        var savedPayment = await _paymentRepository.AddPaymentAsync(payment);

        return _paymentMapper.MapToPaymentDto(savedPayment);
    }

    public async Task<PaymentDto> GetPaymentStatusAsync(int orderId)
    {
        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);
        if (payment == null)
            return null;

        return _paymentMapper.MapToPaymentDto(payment);
    }
}