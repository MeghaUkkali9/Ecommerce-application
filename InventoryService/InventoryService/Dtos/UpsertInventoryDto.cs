namespace InventoryService.Dtos;

public class UpsertInventoryDto
{
    public string? Id { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}