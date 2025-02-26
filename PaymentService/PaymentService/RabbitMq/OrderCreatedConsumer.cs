using MassTransit;
using OrderService.Contracts;
using PaymentService.Data;
using PaymentService.Extensions;
using PaymentService.Services;

namespace PaymentService.RabbitMq;

public class OrderCreatedConsumer : IConsumer<Contracts.OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IPaymentService _paymentService;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger,
        IPaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderCreated> context)
    {
        var message = context.Message;
       
        var paymentDto = await _paymentService.GetPaymentStatusAsync(message.OrderId);

        if (paymentDto != null && paymentDto.PaymentStatus == PaymentStatus.Successful)
        {
            _logger.LogWarning("Payment already completed for Order ID: {OrderId}", message.OrderId);
            return;
        }
        
        var paymentResult = await _paymentService.ProcessPaymentAsync(
            message.OrderId, message.OrderPrice, message.PaymentMethod.MapPaymentMethod());
        
        if (paymentResult.PaymentStatus == PaymentStatus.Successful)
        {
            await context.Publish(new Contracts.PaymentProcessed(message.OrderId, PaymentStatus.Successful.ToString()));
            _logger.LogInformation("Payment successful for Order ID: {OrderId}", message.OrderId);
        }
    }
}