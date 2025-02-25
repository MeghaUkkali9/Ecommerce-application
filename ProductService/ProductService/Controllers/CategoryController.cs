using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDbSharedLibrary;
using ProductService.Data.Models;
using ProductService.Dtos;
using ProductService.Extensions;

namespace ProductService.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;

    public CategoryController(IRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var category = (await _categoryRepository.GetAllAsync())
            .Select(x => x.ToDto());
        return Ok(category);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoryById(string id)
    {
        var category = await _categoryRepository.GetByIdAsync(x => x.Id == new ObjectId(id));
        return Ok(category.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryDto categoryDto)
    {
        var category = new Category()
        {
            Name = categoryDto.CategoryName,
            Description = categoryDto.CategoryDescription
        };
        await _categoryRepository.CreateAsync(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }
}