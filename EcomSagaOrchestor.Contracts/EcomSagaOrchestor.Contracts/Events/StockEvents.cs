namespace EcomSagaOrchestor.Contracts.Events;

public class StockEvents
{
    public record StockReserved(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        int Quantity);

    public record StockReservationFailed(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        int Quantity,
        string Reason);
    
    public record StockNotAvailable(
        Guid CorrelationId,
        int OrderId,
        string ProductId,
        int Quantity);
}