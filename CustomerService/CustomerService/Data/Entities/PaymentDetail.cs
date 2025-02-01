using System.ComponentModel.DataAnnotations;

namespace CustomerService.Data.Entities;

public class PaymentDetail
{
    [Key] 
    public int PaymentId { get; set; }
    public int CustomerId { get; set; }
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string CVV { get; set; }
}