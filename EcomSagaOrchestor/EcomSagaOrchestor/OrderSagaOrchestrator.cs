using EcomSagaOrchestor.Contracts.Commands;
using EcomSagaOrchestor.Contracts.Events;
using MassTransit;

namespace EcomSagaOrchestor;

public class OrderSagaOrchestrator : MassTransitStateMachine<OrderSagaState>
{
    public State OrderCreated { get; private set; }
    public State StockReserved { get; private set; }
    public State PaymentProcessed { get; private set; }
    public State OrderCancelled { get; private set; }
    public State OrderCompleted { get; private set; }
    public Event<OrderEvents.OrderCreated> OrderCreatedEvent { get; private set; }
    public Event<StockEvents.StockReserved> StockReservedEvent { get; private set; }
    public Event<StockEvents.StockNotAvailable> StockNotAvailableEvent { get; private set; }
    public Event<PaymentEvents.PaymentProcessed> PaymentProcessedEvent { get; private set; }
    public Event<PaymentEvents.PaymentFailed> PaymentFailedEvent { get; private set; }
    public Event<OrderEvents.OrderCancelled> OrderCancelledEvent { get; private set; }

    public OrderSagaOrchestrator()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderCreatedEvent, x
            => x.CorrelateById(context => context.Message.CorrelationId));


        Event(() => StockReservedEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        Event(() => StockNotAvailableEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        Event(() => PaymentProcessedEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        Event(() => PaymentFailedEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });
        
        Event(() => OrderCancelledEvent, x =>
        {
            x.CorrelateById(context => context.Message.CorrelationId);
        });

        Initially(
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.OrderPrice = context.Message.OrderPrice;
                    context.Saga.PaymentMethod = context.Message.PaymentMethod;
                    context.Saga.Quantity = context.Message.Quantity;

                    Console.WriteLine(
                        $"Saga started for Order {context.Saga.OrderId} with CorrelationId: {context.Saga.CorrelationId}");
                })
                .Publish(context => new StockCommands.ReserveStock(
                    context.Saga.CorrelationId,
                    context.Saga.OrderId,
                    context.Saga.ProductId,
                    context.Saga.Quantity
                ))
                .TransitionTo(OrderCreated)
        );

        //StockReserved -> proceed with payment
         During(OrderCreated,
             When(StockReservedEvent)
                 .Then(context =>
                 {
                     context.Saga.OrderId = context.Message.OrderId;  
                     Console.WriteLine($"Process payment{context.Saga.OrderId} with CorrelationId: {context.Saga.CorrelationId}");
                 })
                 .Publish(context => new PaymentCommands.ProcessPayment(
                     context.Saga.CorrelationId, 
                     context.Saga.OrderId,
                     context.Saga.OrderPrice,
                     context.Saga.PaymentMethod
                 ))
                 .TransitionTo(StockReserved)
         );
        
        //StockNotAvailable -> cancel order
        During(OrderCreated,
            When(StockNotAvailableEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Stock unavailable for Order {context.Saga.OrderId}, cancelling order.");
                })
                .Publish(context => new OrderCommands.CancelOrder(context.Saga.CorrelationId, context.Saga.OrderId))
                .TransitionTo(OrderCancelled)
        );
        
        // //PaymentSuccess -> complete order
        During(StockReserved,
            When(PaymentProcessedEvent)
                .TransitionTo(PaymentProcessed)
                .Publish(context => new OrderCommands.CompleteOrder(context.Saga.CorrelationId, context.Saga.OrderId))
        );
        
        //PaymentFailure -> cancel order & restore stock
        During(StockReserved,
            When(PaymentFailedEvent)
                .TransitionTo(OrderCancelled)
                .Then(context =>
                {
                    Console.WriteLine($"Payment failed for Order {context.Saga.OrderId}, rolling back inventory.");
                })
                .Publish(context => new StockCommands.RestoreStock(
                    context.Saga.CorrelationId,
                    context.Saga.OrderId,
                    context.Saga.ProductId,
                    context.Saga.Quantity
                ))
                .Publish(context => new OrderCommands.CancelOrder(context.Saga.CorrelationId, context.Saga.OrderId)) 
        );
        
        //Handle order cancellation in any state
        DuringAny(
            When(OrderCancelledEvent)
                .TransitionTo(OrderCancelled)
                .Publish(context => new OrderCommands.CancelOrder(context.Saga.CorrelationId, context.Saga.OrderId))
        );

        SetCompletedWhenFinalized();
    }
}
