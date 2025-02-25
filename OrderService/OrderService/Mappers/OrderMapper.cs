using OrderService.Data.Entities;
using OrderService.Dtos;

namespace OrderService.Mappers;

public interface IOrderMapper
{
    OrderDto MapToOrderDto(Order order);
    Order MapToOrderModel(OrderDto orderDto);
}

public class OrderMapper : IOrderMapper
{
    public OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), order.OrderStatus),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            PaymentStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus),order.PaymentStatus),
            ShippingAddress = order.ShippingAddress,
            ShippingDate = order.ShippingDate
        };
    }
    
    public Order MapToOrderModel(OrderDto orderDto)
    {
        return new Order
        {
            OrderId = orderDto.OrderId,
            CustomerId = orderDto.CustomerId,
            OrderStatus = orderDto.OrderStatus.ToString(),
            TotalAmount = orderDto.TotalAmount,
            OrderDate = orderDto.OrderDate,
            PaymentStatus = orderDto.PaymentStatus.ToString(),
            ShippingAddress = orderDto.ShippingAddress,
            ShippingDate = orderDto.ShippingDate
        };
    }
}
