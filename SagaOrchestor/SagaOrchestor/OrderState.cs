using MassTransit;

namespace SagaOrchestor;

public class OrderState: SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public string OrderId { get; set; }
    public string PaymentId { get; set; }
    public string InventoryId { get; set; }
}