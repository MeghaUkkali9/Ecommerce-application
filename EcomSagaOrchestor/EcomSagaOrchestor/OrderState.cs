using MassTransit;

namespace EcomSagaOrchestor;

public class OrderState: SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }  
    public State CurrentState { get; set; } 
    public int OrderId { get; set; }
    public string ProductId { get; set; }
    public decimal OrderPrice { get; set; }
    public string PaymentMethod { get; set; }
    public int Quantity { get; set; }
}