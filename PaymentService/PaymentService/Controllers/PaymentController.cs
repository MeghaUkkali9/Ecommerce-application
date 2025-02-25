using Microsoft.AspNetCore.Mvc;
using PaymentService.Data;
using PaymentService.Dtos;
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
        var paymentResult =
            await _paymentService.ProcessPaymentAsync(orderId, paymentDto.Amount,
                MapPaymentMethod(paymentDto.PaymentMethod));

        if (paymentResult == null)
        {
            throw new Exception("Payment processing failed. Please try again");
        }
        return Ok(paymentResult);
    }

    [HttpGet("status/{orderId}")]
    public async Task<IActionResult> GetPaymentStatus(int orderId)
    {
        var paymentStatus = await _paymentService.GetPaymentStatusAsync(orderId);

        return Ok(paymentStatus);
    }
    
    private static PaymentMethod MapPaymentMethod(string paymentMethod)
    {
        return paymentMethod switch
        {
            "credit" => PaymentMethod.CreditCard,
            "debit" => PaymentMethod.DebitCard,
            "cash" => PaymentMethod.CashOnDelivery,
            _ => throw new NotSupportedException("Unsupported payment method.")
        };
    }
}