using Newtonsoft.Json;

namespace InventoryService.Proxies;

public interface IProductProxy
{
    Task<ProductDto> GetProductById(string productId);
}

public class ProductProxy : IProductProxy
{
    private readonly HttpClient _httpClient;

    public ProductProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductDto> GetProductById(string productId)
    {
        var response = await _httpClient.GetAsync($"api/products/{productId}");
    
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<ProductDto>(jsonString);
    }
}