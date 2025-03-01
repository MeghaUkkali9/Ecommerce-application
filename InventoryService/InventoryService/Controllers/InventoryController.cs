using InventoryService.Dtos;
using InventoryService.Extensions;
using InventoryService.Proxies;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace InventoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IProductProxy _productProxy;

    public InventoryController(
        IProductProxy productProxy,
        IInventoryService inventoryService)
    {
        _productProxy = productProxy;
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
    {
        var inventory = await _inventoryService.GetAll();
        return Ok(inventory.Select(x => x.ToDto()));
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductInventory>> GetByProductId(string productId)
    {
        var inventory = (await _inventoryService.Get(productId)).ToDto();

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
        if (!ObjectId.TryParse(inventory.ProductId, out var productObjectId))
        {
            return BadRequest("Invalid Product ID format.");
        }

        var productInventory = (await _inventoryService.Upsert(productObjectId, inventory)).ToDto();

        return CreatedAtAction(nameof(GetByProductId),
            new { productId = productInventory.ProductId }, productInventory);
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult> UpdateInventory(string productId, [FromBody] UpdateInventoryDto inventory)
    {
        if (!ObjectId.TryParse(productId, out var productObjectId))
        {
            return BadRequest("Invalid Product ID format.");
        }

        await _inventoryService.Update(productObjectId, inventory.Quantity);
        return Ok();
    }
}