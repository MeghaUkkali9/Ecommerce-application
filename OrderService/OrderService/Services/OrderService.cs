using EcomSagaOrchestor.Contracts.Events;
using MassTransit;
using OrderService.Dtos;
using OrderService.Mappers;
using OrderService.Proxies;
using OrderService.Repositories;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderDto?> GetOrderById(int id);
    Task<OrderDto> CreateOrder(OrderDto createOrder);
    Task<OrderDto?> UpdateOrder(int id, OrderDto updateOrder);
    Task<bool> DeleteOrder(int id);
    Task<OrderDto> ProcessOrder(OrderDto orderDto, string productId, string paymentMethod, int quantity);
    Task UpdateOrder(OrderDto order);
}
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMapper _orderMapper;
    private readonly IProductProxy _productProxy;
    private readonly ICustomerProxy _customerProxy;
    private readonly IPriceCalculator _priceCalculator;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IOrderRepository orderRepository, 
        IOrderMapper orderMapper,
        IProductProxy productProxy,
        ICustomerProxy customerProxy, 
        IPriceCalculator priceCalculator,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _orderMapper = orderMapper;
        _productProxy = productProxy;
        _customerProxy = customerProxy;
        _priceCalculator = priceCalculator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderDto?> GetOrderById(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId); 
        return order != null ? _orderMapper.MapToOrderDto(order) : null;
    }

    public async Task<OrderDto> CreateOrder(OrderDto orderDto)
    {
        var order = _orderMapper.MapToOrderModel(orderDto);
        var createdOrder = await _orderRepository.AddAsync(order); 
        return _orderMapper.MapToOrderDto(createdOrder);
    }

    public async Task<OrderDto?> UpdateOrder(int orderId, OrderDto updateOrderRequest)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(orderId); 
        if (existingOrder == null) return null;

        existingOrder.OrderStatus = updateOrderRequest.OrderStatus;
        existingOrder.ShippingAddress = updateOrderRequest.ShippingAddress;
        existingOrder.ShippingDate = updateOrderRequest.ShippingDate;

        var updatedOrder = await _orderRepository.UpdateAsync(existingOrder); 
        return _orderMapper.MapToOrderDto(updatedOrder);
    }

    public async Task<bool> DeleteOrder(int orderId)
    {
        return await _orderRepository.DeleteAsync(orderId); 
    }

    public async Task<OrderDto> ProcessOrder(OrderDto request, string productId, string paymentMethod, int quantity)
    {
        // var isCustomerRegistered = await _customerProxy.IsCustomerRegistered(request.CustomerId);
        // if (!isCustomerRegistered)
        // {
        //     throw new ArgumentException("Customer must be registered");
        // }

        var product = await _productProxy.GetProductById(productId);
        var totalPrice = _priceCalculator.CalculatePrice(quantity, product.Price);
        
        var order = await CreateOrder(request);
        var correlationId = Guid.NewGuid();
        
        await _publishEndpoint.Publish(new OrderEvents.OrderCreated(correlationId,
            order.OrderId,
            product.Id, totalPrice, paymentMethod, quantity));
        return order;
    }

    public async Task UpdateOrder(OrderDto orderDto)
    {
        var existingOrder = await _orderRepository.GetByIdAsync(orderDto.OrderId);
        existingOrder.OrderStatus = orderDto.OrderStatus;
        await _orderRepository.UpdateAsync(existingOrder); 
    }
}
