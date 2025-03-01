using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Entities;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShippingAddress { get; set; }
    public DateTime ShippingDate { get; set; }
}