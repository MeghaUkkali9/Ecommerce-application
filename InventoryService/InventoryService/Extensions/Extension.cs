using InventoryService.Data;
using InventoryService.Dtos;

namespace InventoryService.Extensions;

public static class Extension
{
    public static InventoryDto ToDto(this Inventory inventoryDto)
    {
        return new InventoryDto()
        {
            Id = inventoryDto.Id.ToString(),
            ProductId = inventoryDto.ProductId.ToString(),
            Quantity = inventoryDto.Quantity
        };
    }
}