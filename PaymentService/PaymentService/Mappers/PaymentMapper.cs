using PaymentService.Data;
using PaymentService.Dtos;

namespace PaymentService.Mappers;

public interface IPaymentMapper
{
    PaymentDto MapToPaymentDto(Payment payment);
    Payment MapToPaymentModel(PaymentDto paymentDto);
}

public class PaymentMapper : IPaymentMapper
{
    public PaymentDto MapToPaymentDto(Payment payment)
    {
        if (payment is null)
        {
            return null;
        }

        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod 
        };
    }

    public Payment MapToPaymentModel(PaymentDto paymentDto)
    {
        return new Payment
        {
            OrderId = paymentDto.OrderId,
            Amount = paymentDto.Amount,
            PaymentStatus = paymentDto.PaymentStatus,
            PaymentDate = paymentDto.PaymentDate,
            PaymentMethod = paymentDto.PaymentMethod
        };
    }
}