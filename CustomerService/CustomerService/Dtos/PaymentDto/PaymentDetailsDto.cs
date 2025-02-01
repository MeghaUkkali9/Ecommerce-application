namespace CustomerService.DTOs;

public class PaymentDetailsDto
{
    public int PaymentId { get; set; }
    public int CustomerId { get; set; }
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string CVV { get; set; }
}