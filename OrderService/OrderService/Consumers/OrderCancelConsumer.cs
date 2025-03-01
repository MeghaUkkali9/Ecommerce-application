using EcomSagaOrchestor.Contracts.Commands;
using MassTransit;
using OrderService.Services;

namespace OrderService.Consumers;

public class OrderCancelConsumer : IConsumer<OrderCommands.CancelOrder>
{
    private readonly IOrderService _orderService;

    public OrderCancelConsumer(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<OrderCommands.CancelOrder> context)
    {
        var orderId = context.Message.OrderId;
        
        var order = await _orderService.GetOrderById(orderId);

        if (order != null)
        {
            order.OrderStatus = "Cancelled";
            await _orderService.UpdateOrder(order);
            Console.WriteLine(
                $"Order {orderId} is Cancelled for Correlation id:{context.Message.CorrelationId}.");
        }
    }
}