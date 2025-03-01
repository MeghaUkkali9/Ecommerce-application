namespace OrderService.Dtos;

public class CreateOrder
{
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShippingAddress { get; set; }
    public DateTime ShippingDate { get; set; }
}