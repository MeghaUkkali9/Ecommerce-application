using EcomSagaOrchestor.Contracts.Commands;
using InventoryService.Services;
using MassTransit;
using MongoDB.Bson;

namespace InventoryService.Consumers;

public class RestoreStockConsumer : IConsumer<StockCommands.RestoreStock>
{
    private readonly IInventoryService _inventoryService;

    public RestoreStockConsumer(
        IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task Consume(ConsumeContext<StockCommands.RestoreStock> context)
    {
        var productId = context.Message.ProductId;
        var orderId = context.Message.OrderId.ToString();
        
        var stock = await _inventoryService.Get(productId);

        if (stock != null)
        {
            stock.Quantity += context.Message.Quantity;
            await _inventoryService.Update(new ObjectId(productId), stock.Quantity);

            Console.WriteLine($"After Stock reserved for Order {orderId}: {stock.Quantity}");
        }
    }
}