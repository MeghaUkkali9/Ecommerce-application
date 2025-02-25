namespace ProductService.Dtos;

public class ProductDto
{
    public string Id { get; set; }
    public string CategoryId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ProductDescription { get; set; }
}