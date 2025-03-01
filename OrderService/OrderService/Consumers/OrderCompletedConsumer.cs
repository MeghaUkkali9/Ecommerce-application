using EcomSagaOrchestor.Contracts.Commands;
using MassTransit;
using OrderService.Services;

namespace OrderService.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCommands.CompleteOrder>
{
    private readonly IOrderService _orderService;

    public OrderCompletedConsumer(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<OrderCommands.CompleteOrder> context)
    {
        var orderId = context.Message.OrderId;
        var order = await _orderService.GetOrderById(orderId);

        if (order != null)
        {
            order.OrderStatus = "Completed";
            await _orderService.UpdateOrder(order);
            Console.WriteLine($"Order {orderId} is now completed for Correlation id:{context.Message.CorrelationId}");
        }
    }
}