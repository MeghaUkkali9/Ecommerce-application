using System.Text.Json.Serialization;
using PaymentService.Data;

namespace PaymentService.Dtos;

public class CreatePayment
{
    public decimal Amount { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; }
}