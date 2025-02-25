namespace InventoryService.Dtos;

public class ProductInventory
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string CategoryId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ProductDescription { get; set; }
    public int Quantity { get; set; }
}