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
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            OrderDate = order.OrderDate,
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
            OrderDate = orderDto.OrderDate,
            ShippingAddress = orderDto.ShippingAddress,
            ShippingDate = orderDto.ShippingDate
        };
    }
}
