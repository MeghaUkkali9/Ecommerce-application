namespace EcomSagaOrchestor.Contracts.Commands;

public class PaymentCommands
{
    public record ProcessPayment(
        Guid CorrelationId,
        int OrderId,
        decimal OrderPrice,
        string PaymentMethod);
}