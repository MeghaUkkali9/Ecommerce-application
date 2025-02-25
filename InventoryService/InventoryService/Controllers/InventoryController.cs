using InventoryService.Data;
using InventoryService.Dtos;
using InventoryService.Extensions;
using InventoryService.Proxies;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDbSharedLibrary;

namespace InventoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IRepository<Inventory> _repository;
    private readonly IProductProxy _productProxy;

    public InventoryController(IRepository<Inventory> repository, 
        IProductProxy productProxy)
    {
        _repository = repository;
        _productProxy = productProxy;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
    {
        var inventory =  (await _repository.GetAllAsync()).Select(x=> x.ToDto());
        return Ok(inventory);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductInventory>> GetByProductId(string productId)
    {
        var inventory = (await _repository.GetByIdAsync(x => x.ProductId == new ObjectId(productId)))
            .ToDto();

        if (inventory == null)
        {
            return NotFound();
        }
        var productDto = await _productProxy.GetProductById(inventory.ProductId);

        if (productDto == null)
        {
            return NotFound();
        }
        
        var productInventory = new ProductInventory()
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductName = productDto.ProductName,
            ProductDescription = productDto.ProductDescription,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
            Quantity = inventory.Quantity,
        };
        
        return Ok(productInventory);
    }

    [HttpPost]
    public async Task<ActionResult> UpsertInventory([FromBody] UpsertInventoryDto inventory)
    {
        if (!ObjectId.TryParse(inventory.ProductId, out ObjectId productObjectId))
        {
            return BadRequest("Invalid Product ID format.");
        }

        var existingInventory = await _repository.GetByIdAsync(x => x.ProductId == productObjectId);

        if (existingInventory is not null)
        {
            existingInventory.Quantity += inventory.Quantity;
            await _repository.UpdateAsync(existingInventory);
            return Ok(existingInventory);
        }

        var newInventory = new Inventory()
        {
            ProductId = new ObjectId(inventory.ProductId),
            Quantity = inventory.Quantity
        };
        await _repository.CreateAsync(newInventory);

        return CreatedAtAction(nameof(GetByProductId), new { productId = newInventory.ProductId }, newInventory);
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult> UpdateInventory(string productId, [FromBody] UpdateInventoryDto inventory)
    {
        if (!ObjectId.TryParse(productId, out ObjectId productObjectId))
        {
            return BadRequest("Invalid Product ID format.");
        }

        var existingInventory = await _repository.GetByIdAsync(x => x.ProductId == productObjectId);

        if (existingInventory == null)
        {
            return NotFound("Inventory item not found.");
        }

        existingInventory.Quantity = inventory.Quantity;
        await _repository.UpdateAsync(existingInventory);

        return Ok(existingInventory);
    }
}