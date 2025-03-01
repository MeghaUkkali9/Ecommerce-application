using InventoryService.Data;
using InventoryService.Dtos;
using MongoDB.Bson;
using MongoDbSharedLibrary;

namespace InventoryService.Services;

public interface IInventoryService
{
    Task Update(ObjectId productObjectId, int inventoryQuantity);
    Task<Inventory> Upsert(ObjectId productObjectId, UpsertInventoryDto inventory);
    Task<Inventory> Get(string productId);
    Task<List<Inventory>> GetAll();
}

public class InventoryService : IInventoryService
{
    private readonly IRepository<Inventory> _repository;

    public InventoryService(IRepository<Inventory> repository)
    {
        _repository = repository;
    }

    public async Task Update(ObjectId productObjectId, int inventoryQuantity)
    {
        var existingInventory =
            await _repository.GetByIdAsync(x => x.ProductId == productObjectId);

        if (existingInventory == null)
        {
            throw new ArgumentNullException($"Inventory item for id :{productObjectId} is not found.");
        }

        existingInventory.Quantity = inventoryQuantity;
        await _repository.UpdateAsync(existingInventory);
    }

    public async Task<Inventory> Upsert(ObjectId productObjectId, UpsertInventoryDto inventory)
    {
        var existingInventory = await _repository.GetByIdAsync(x => x.ProductId == productObjectId);

        if (existingInventory is not null)
        {
            existingInventory.Quantity += inventory.Quantity;
            await _repository.UpdateAsync(existingInventory);
            return (existingInventory);
        }

        var newInventory = new Inventory()
        {
            ProductId = new ObjectId(inventory.ProductId),
            Quantity = inventory.Quantity
        };
        await _repository.CreateAsync(newInventory);
        
        return newInventory;
    }

    public async Task<Inventory> Get(string productId)
    {
        return await _repository.GetByIdAsync(x => x.ProductId == new ObjectId(productId));
    }

    public async Task<List<Inventory>> GetAll()
    {
        return await _repository.GetAllAsync();
    }
}