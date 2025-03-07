using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Extensions;
using PaymentService.Services;

namespace PaymentService.Controllers;

[Route("[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("{orderId}")]
    public async Task<IActionResult> ProcessPayment(int orderId, [FromBody] CreatePayment paymentDto)
    {
        if (paymentDto == null)
        {
            return BadRequest("Invalid payment data.");
        }

        var paymentGatewayResponse = PaymentStatus.Successful;
        
        var paymentResult =
            await _paymentService.ProcessPaymentAsync(orderId, paymentDto.Amount,
                paymentDto.PaymentMethod.MapPaymentMethod(), paymentGatewayResponse);

        if (paymentResult == null)
        {
            throw new Exception("Payment processing failed. Please try again");
        }
        return Ok(paymentResult);
    }

    [HttpGet("status/{orderId}")]
    public async Task<ActionResult<PaymentDto>> GetPaymentStatus(int orderId)
    {
        var paymentDto = await _paymentService.GetPaymentStatusAsync(orderId);

        return Ok(paymentDto);
    }
}