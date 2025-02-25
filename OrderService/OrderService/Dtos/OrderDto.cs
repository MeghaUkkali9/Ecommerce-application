namespace OrderService.Dtos;

public class OrderDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string ShippingAddress { get; set; }
    public DateTime ShippingDate { get; set; }
}
