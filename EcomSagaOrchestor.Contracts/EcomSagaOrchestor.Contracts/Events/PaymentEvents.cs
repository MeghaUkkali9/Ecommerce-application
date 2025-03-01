namespace EcomSagaOrchestor.Contracts.Events;

public class PaymentEvents
{
    public record PaymentProcessed(
        Guid CorrelationId,
        int OrderId,
        decimal OrderPrice,
        string PaymentMethod);

    public record PaymentFailed(
        int OrderId,
        Guid CorrelationId,
        decimal OrderPrice,
        string PaymentMethod,
        string Reason);
}