using EcomSagaOrchestor.Contracts.Commands;
using EcomSagaOrchestor.Contracts.Events;
using MassTransit;

namespace EcomSagaOrchestor;

public class OrderSaga : MassTransitStateMachine<OrderState>
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

    public OrderSaga()
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

        //StockReserved -> Proceed to Payment
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
        
        //StockNotAvailable -> Cancel Order
        During(OrderCreated,
            When(StockNotAvailableEvent)
                .Then(context =>
                {
                    Console.WriteLine($"Stock unavailable for Order {context.Saga.OrderId}, cancelling order.");
                })
                .Publish(context => new OrderCommands.CancelOrder(context.Saga.CorrelationId, context.Saga.OrderId))
                .TransitionTo(OrderCancelled)
        );
        
        // //PaymentSuccess -> Complete Order
        During(StockReserved,
            When(PaymentProcessedEvent)
                .TransitionTo(PaymentProcessed)
                .Publish(context => new OrderCommands.CompleteOrder(context.Saga.CorrelationId, context.Saga.OrderId))
        );
        
        //PaymentFailure -> Cancel Order & Restore Stock
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
        
        //Handle Order Cancellation in Any State
        DuringAny(
            When(OrderCancelledEvent)
                .TransitionTo(OrderCancelled)
                .Publish(context => new OrderCommands.CancelOrder(context.Saga.CorrelationId, context.Saga.OrderId))
        );

        SetCompletedWhenFinalized();
    }
}
