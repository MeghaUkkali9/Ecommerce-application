using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrder createOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var orderDto = new OrderDto()
            {
                CustomerId = createOrder.CustomerId,
                OrderStatus = OrderStatus.Pending,
                TotalAmount = createOrder.TotalAmount,
                OrderDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = createOrder.ShippingAddress,
                ShippingDate = createOrder.ShippingDate
            };

            var createdOrder = await _orderService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrder),
                new { id = createdOrder.OrderId }, createdOrder);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> PlaceOrder([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Make sure customer id exists in the ecomcustomerdb.
            // Validate that the customer exists (for this, you would need to call the customer service or repository)
            await _orderService.ProcessOrder(request);
            var orderDto = new OrderDto()
            {
                CustomerId = createOrder.CustomerId,
                OrderStatus = OrderStatus.Pending,
                TotalAmount = createOrder.TotalAmount,
                OrderDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = createOrder.ShippingAddress,
                ShippingDate = createOrder.ShippingDate
            };

            var createdOrder = await _orderService.CreateOrder(orderDto);
            return CreatedAtAction(nameof(GetOrder),
                new { id = createdOrder.OrderId }, createdOrder);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrder updateOrderRequest)
        {
            var orderDto = new OrderDto()
            {
                CustomerId = updateOrderRequest.CustomerId,
                OrderStatus = OrderStatus.Pending,
                TotalAmount = updateOrderRequest.TotalAmount,
                OrderDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Pending,
                ShippingAddress = updateOrderRequest.ShippingAddress,
                ShippingDate = updateOrderRequest.ShippingDate
            };

            var updatedOrder = await _orderService.UpdateOrder(id, orderDto);
            if (updatedOrder == null)
            {
                return NotFound();
            }

            return Ok(updatedOrder);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var isDeleted = await _orderService.DeleteOrder(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
