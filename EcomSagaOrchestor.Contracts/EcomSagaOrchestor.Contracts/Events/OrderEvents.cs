namespace EcomSagaOrchestor.Contracts.Events;

public class OrderEvents
{
    public record OrderCreated(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        decimal OrderPrice,
        string PaymentMethod,
        int Quantity);

    public record OrderCancelled(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        decimal OrderPrice,
        string Reason);
}