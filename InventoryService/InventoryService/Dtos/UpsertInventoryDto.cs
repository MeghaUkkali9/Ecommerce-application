namespace InventoryService.Dtos;

public class UpsertInventoryDto
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}