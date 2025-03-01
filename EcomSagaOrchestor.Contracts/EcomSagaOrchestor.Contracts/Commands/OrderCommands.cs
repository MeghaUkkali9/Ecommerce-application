namespace EcomSagaOrchestor.Contracts.Commands;

public class OrderCommands
{
    public record CancelOrder(
        Guid CorrelationId,
        int OrderId);
    
    public record CompleteOrder(
        Guid CorrelationId,
        int OrderId);
}