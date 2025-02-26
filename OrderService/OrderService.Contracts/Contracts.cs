namespace OrderService.Contracts;

public class Contracts
{
    public record OrderCreated(
        int OrderId,
        decimal OrderPrice,
        string PaymentMethod
        );
    
    public record OrderUpdated(
        int OrderId,
        int CustomerId, 
        DateTime OrderDate,
        decimal OrderPrice,
        string OrderStatus,
        string PaymentStatus,
        string ShippingAddress,
        DateTime ShippedDate
    );
    
    public record OrderDeleted(
        int OrderId
    );
    
    public record PaymentProcessed(int OrderId, string PaymentStatus);
}