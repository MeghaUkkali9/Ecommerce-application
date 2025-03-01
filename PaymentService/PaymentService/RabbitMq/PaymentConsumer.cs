using EcomSagaOrchestor.Contracts.Commands;
using EcomSagaOrchestor.Contracts.Events;
using MassTransit;
using PaymentService.Data;
using PaymentService.Extensions;
using PaymentService.Services;

namespace PaymentService.RabbitMq;

public class PaymentConsumer : IConsumer<PaymentCommands.ProcessPayment>
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentConsumer> _logger;

    public PaymentConsumer(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task Consume(ConsumeContext<PaymentCommands.ProcessPayment> context)
    {
        var orderId = context.Message.OrderId;
        var amount = context.Message.OrderPrice;
        var paymentMethod = context.Message.PaymentMethod;

        var paymentDto = await _paymentService.GetPaymentStatusAsync(orderId);

        if (paymentDto != null && paymentDto.PaymentStatus == PaymentStatus.Successful)
        {
            _logger.LogWarning("Payment already completed for Order ID: {OrderId}", orderId);
            context.Publish(new PaymentEvents.PaymentProcessed(
                context.Message.CorrelationId,
                orderId,
                amount,
                paymentMethod));
        }

        Console.WriteLine($"Processing payment for Order {orderId} of amount {amount} using {paymentMethod}");
        //Get response from payment gateway, hardcoding to succesfull.
        PaymentStatus paymentGatewaySuccesful = PaymentStatus.Successful;

        if (paymentGatewaySuccesful == PaymentStatus.Successful)
        {
            await _paymentService.ProcessPaymentAsync(
                orderId, amount, paymentMethod.MapPaymentMethod(), paymentGatewaySuccesful);

            Console.WriteLine($"Payment successful for Order {orderId}");
            await context.Publish(new PaymentEvents.PaymentProcessed(
                context.Message.CorrelationId,
                orderId,
                amount,
                paymentMethod));
        }
        else
        {
            await _paymentService.ProcessPaymentAsync(
                orderId, amount, paymentMethod.MapPaymentMethod(), paymentGatewaySuccesful);
            
            Console.WriteLine($"Payment failed for Order {orderId}");

            await context.Publish(new PaymentEvents.PaymentFailed(
                orderId,
                context.Message.CorrelationId,
                amount,
                paymentMethod,
                "Payment Gateway Error"));
        }
    }
}