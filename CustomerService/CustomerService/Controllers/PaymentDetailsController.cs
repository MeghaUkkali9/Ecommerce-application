using CustomerService.Data.Entities;
using CustomerService.DTOs;
using CustomerService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/paymentdetails")]
public class PaymentDetailsController : ControllerBase
{
    private readonly IPaymentDetailsRepository _paymentDetailsRepository;
    private readonly ILogger<PaymentDetailsController> _logger;
    
    public PaymentDetailsController(IPaymentDetailsRepository paymentDetailsRepository,
        ILogger<PaymentDetailsController> logger)
    {
        _paymentDetailsRepository = paymentDetailsRepository;
        _logger = logger;
    }

    [HttpGet("{customerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PaymentDetailsDto> GetPaymentDetails(int customerId)
    {
        try
        {
            var paymentDetails = _paymentDetailsRepository.GetPaymentDetails(customerId);
            if (paymentDetails == null)
            {
                return NotFound();
            }

            return Ok(paymentDetails);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occurred while retrieving payment details for customer ID {customerId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PaymentDetailsDto> CreatePaymentDetails([FromBody] CreatePaymentDetailsDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Bad Request");
            }

            var paymentDetail = new PaymentDetail()
            {
                CustomerId = request.CustomerId,
                CardHolderName = request.CardHolderName,
                CardNumber = request.CardNumber,
                CVV = request.CVV,
                ExpiryDate = request.ExpiryDate
            };
            var createdPaymentDetails = _paymentDetailsRepository.CreatePaymentDetails(paymentDetail);
            return CreatedAtAction(nameof(GetPaymentDetails),
                new { customerId = createdPaymentDetails.CustomerId }, createdPaymentDetails);
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, $"Error occurred while saving payment details for customer ID {request.CustomerId}");
            return StatusCode(500, "Internal server error while saving payment details");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Unexpected error occurred while creating payment details for customer ID {request.CustomerId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{customerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeletePaymentDetails(int customerId)
    {
        try
        {
            var result = _paymentDetailsRepository.DeletePaymentDetails(customerId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, $"Error occurred while deleting payment details for customer ID {customerId}");
            return StatusCode(500, "Internal server error while deleting payment details");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Unexpected error occurred while deleting payment details for customer ID {customerId}");
            return StatusCode(500, "Internal server error");
        }
    }
}