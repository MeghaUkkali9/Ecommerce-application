using Newtonsoft.Json;
using OrderService.Proxies.Dtos;

namespace OrderService.Proxies;

public interface ICustomerProxy
{
    Task<bool> IsCustomerRegistered(string customerId);
}

public class CustomerProxy : ICustomerProxy
{
    private readonly HttpClient _httpClient;

    public CustomerProxy(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsCustomerRegistered(string customerId)
    {
       var response = await _httpClient.GetAsync("api/customers/" + customerId);
       if (!response.IsSuccessStatusCode)
       {
           return false;
       }

       var jsonString = await response.Content.ReadAsStringAsync();
       var customer = JsonConvert.DeserializeObject<CustomerDto>(jsonString);
       
       if (customer is null)
       {
           return false;
       }

       return true;
    }
}