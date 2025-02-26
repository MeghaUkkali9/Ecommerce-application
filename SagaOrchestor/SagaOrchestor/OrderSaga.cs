using MassTransit;

namespace SagaOrchestor;

public class OrderSaga : MassTransitStateMachine<OrderState>
{
    public State PaymentPending { get; private set; }
    public State InventoryPending { get; private set; }
    public State OrderCompleted { get; private set; }
    public State OrderCanceled { get; private set; }
    public Event<OrderCreated> OrderCreatedEvent { get; private set; }
    public Event<PaymentSuccessful> PaymentSuccessfulEvent { get; private set; }
    public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
    public Event<InventoryReserved> InventoryReservedEvent { get; private set; }
    public Event<InventoryFailed> InventoryFailedEvent { get; private set; }

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreatedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentSuccessfulEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => InventoryReservedEvent, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => InventoryFailedEvent, x => x.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    Console.WriteLine($"Saga started for Order {context.Saga.OrderId}");
                })
                .TransitionTo(PaymentPending)
                .Publish(context => new ProcessPayment
                {
                    OrderId = context.Saga.OrderId,
                    Amount = context.Message.TotalAmount
                })
        );

        During(PaymentPending,
            When(PaymentSuccessfulEvent)
                .TransitionTo(InventoryPending)
                .Publish(context => new ReserveInventory
                {
                    OrderId = context.Saga.OrderId
                }),

            When(PaymentFailedEvent)
                .TransitionTo(OrderCanceled)
                .Publish(context => new CancelOrder
                {
                    OrderId = context.Saga.OrderId
                })
        );

        During(InventoryPending,
            When(InventoryReservedEvent)
                .TransitionTo(OrderCompleted)
                .Then(context =>
                {
                    Console.WriteLine($"Order {context.Saga.OrderId} successfully completed!");
                }),

            When(InventoryFailedEvent)
                .TransitionTo(OrderCanceled)
                .Publish(context => new RefundPayment
                {
                    OrderId = context.Saga.OrderId,
                    PaymentId = context.Saga.PaymentId
                })
        );

        SetCompletedWhenFinalized();
}