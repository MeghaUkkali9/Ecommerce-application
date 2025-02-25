using MongoDB.Bson;
using MongoDbSharedLibrary;
using ProductService.Data.Models;
using ProductService.Dtos;
using ProductService.Extensions;

namespace ProductService.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productRepository;

    public ProductsController(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = (await _productRepository.GetAllAsync())
            .Select(product => product.ToDto());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(string id)
    {
        var product = (await _productRepository.GetByIdAsync(id)).ToDto();
        if (product == null)
        {
            return NotFound();
        }

        
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(CreateProduct productDto)
    {
        if (productDto == null)
        {
            return BadRequest("Product data is required.");
        }
        
        var product = new Product
        {
            CategoryId = new ObjectId(productDto.CategoryId),
            Name = productDto.ProductName,
            Description = productDto.ProductDescription,
            Price = productDto.Price
        };

        await _productRepository.CreateAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(string id, UpdateProductDto product)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Id = new ObjectId(id);
        existingProduct.CategoryId = new ObjectId(product.CategoryId);
        existingProduct.Name = product.ProductName;
        existingProduct.Description = product.ProductDescription;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.ProductDescription;

        await _productRepository.UpdateAsync(existingProduct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productRepository.DeleteAsync(id);
        return NoContent();
    }
}