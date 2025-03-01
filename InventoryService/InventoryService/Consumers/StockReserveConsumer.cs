using EcomSagaOrchestor.Contracts.Commands;
using EcomSagaOrchestor.Contracts.Events;
using InventoryService.Services;
using MassTransit;
using MongoDB.Bson;

namespace InventoryService.Consumers;

public class StockReserveConsumer: IConsumer<StockCommands.ReserveStock>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IInventoryService _inventoryService;
    
    public StockReserveConsumer(IPublishEndpoint publishEndpoint, 
        IInventoryService inventoryService)
    {
        _publishEndpoint = publishEndpoint;
        _inventoryService = inventoryService;
    }

    public async Task Consume(ConsumeContext<StockCommands.ReserveStock> context)
    {
        var stock = await _inventoryService.Get(context.Message.ProductId);

        if (stock != null && stock.Quantity >= context.Message.Quantity)
        {
            Console.WriteLine($"Before Stock available for Order {stock.Quantity}");
            stock.Quantity -= context.Message.Quantity;
            
            await _inventoryService.Update(new ObjectId(context.Message.ProductId), stock.Quantity);
            
            Console.WriteLine($"AfterStock reserved for Order {context.Message.OrderId.ToString()}: {stock.Quantity}");

            await _publishEndpoint.Publish(new StockEvents.StockReserved(context.Message.CorrelationId,
                context.Message.OrderId, context.Message.ProductId, context.Message.Quantity));
        }
        else
        {
            Console.WriteLine($"Stock not available for Order {context.Message.OrderId.ToString()}");

            await _publishEndpoint.Publish(new StockEvents.StockNotAvailable(context.Message.CorrelationId,
                context.Message.OrderId, context.Message.ProductId, context.Message.Quantity));
        }
    }
}