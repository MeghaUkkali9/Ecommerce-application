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
    Task ProcessOrder(OrderRequest request);
}
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderMapper _orderMapper;
    private readonly IProductProxy _productProxy;
    private readonly ICustomerProxy _customerProxy;
    private readonly IPriceCalculator _priceCalculator;

    public OrderService(IOrderRepository orderRepository, 
        IOrderMapper orderMapper,
        IProductProxy productProxy,
        ICustomerProxy customerProxy, 
        IPriceCalculator priceCalculator)
    {
        _orderRepository = orderRepository;
        _orderMapper = orderMapper;
        _productProxy = productProxy;
        _customerProxy = customerProxy;
        _priceCalculator = priceCalculator;
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

        existingOrder.OrderStatus = updateOrderRequest.OrderStatus.ToString();
        existingOrder.TotalAmount = updateOrderRequest.TotalAmount;
        existingOrder.PaymentStatus = updateOrderRequest.PaymentStatus.ToString();
        existingOrder.ShippingAddress = updateOrderRequest.ShippingAddress;
        existingOrder.ShippingDate = updateOrderRequest.ShippingDate;

        var updatedOrder = await _orderRepository.UpdateAsync(existingOrder); 
        return _orderMapper.MapToOrderDto(updatedOrder);
    }

    public async Task<bool> DeleteOrder(int orderId)
    {
        return await _orderRepository.DeleteAsync(orderId); 
    }

    public async Task ProcessOrder(OrderRequest request)
    {
        var isCustomerRegistered = await _customerProxy.IsCustomerRegistered(request.CustomerId);
        if (!isCustomerRegistered)
        {
            throw new ArgumentException("Customer must be registered");
        }

        var product = await _productProxy.GetProductById(request.ProductId);
        var totalPrice = _priceCalculator.CalculatePrice(request.Quantity, product.Price);
        
        
        //if payment is successful then only order is placed.
    }
}
