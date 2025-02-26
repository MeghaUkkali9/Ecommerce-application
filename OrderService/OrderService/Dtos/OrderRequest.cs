namespace OrderService.Dtos;

public class OrderRequest
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; }
}