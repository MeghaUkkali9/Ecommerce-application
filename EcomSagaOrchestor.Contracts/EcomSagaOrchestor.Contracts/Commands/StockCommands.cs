namespace EcomSagaOrchestor.Contracts.Commands;

public class StockCommands
{
    public record ReserveStock(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        int Quantity);
    
    public record RestoreStock(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        int Quantity);
}
