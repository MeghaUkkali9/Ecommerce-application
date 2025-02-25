using MongoDB.Bson;
using ProductService.Data.Models;
using ProductService.Dtos;

namespace ProductService.Extensions;

public static class Extension
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto()
        {
            Id = product.Id.ToString(),
            ProductName = product.Name,
            CategoryId = product.CategoryId.ToString(),
            Price = product.Price,
            ProductDescription = product.Description
        };
    }
    
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto()
        {
           CategoryId = category.Id.ToString(),
           CategoryName = category.Name,
           CategoryDescription = category.Description
        };
    }
}