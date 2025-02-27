To work with RabbitMQ:
1. Install Required Packages: In each service that interacts with RabbitMQ 
    <PackageReference Include="MassTransit.AspNetCore" Version="7.3.1" />
    <PackageReference Include="MassTransit.RabbitMQ" Version="8.3.6" />
    
2. Define Configuration in appsettings.json
    "ServiceSetting": {
        "ServiceName": "order"
      },
      "RabbitMqSetting":{
        "Host": "localhost"
      }
      
3. Configure RabbitMQ in Program.cs
        public static IServiceCollection AddRabbitMqWithMassTransit(this IServiceCollection service)
            {
                service.AddMassTransit(configure =>
                {
                    /*
                     * This scans the current executing assembly for any classes that
                     * implement IConsumer<T> and automatically registers them.
                     */
                    configure.AddConsumers(Assembly.GetEntryAssembly());
                    
                    configure.UsingRabbitMq((context, configurator) =>
                        {
                            var configuration = context.GetService<IConfiguration>();
                            var serviceSetting = configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
                            var rabbitMqSetting = configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>();
                            
                            configurator.Host(rabbitMqSetting.Host); //This line tells MassTransit where RabbitMQ is running
                            
                            //This sets up queues and exchanges using a kebab-case naming strategy.
                            configurator.ConfigureEndpoints(context,
                                new KebabCaseEndpointNameFormatter(serviceSetting.ServiceName, false));
                        }
                    );
                });
               
                return service;
            }

4. Publishing Messages (Producer): Use IPublishEndpoint to send messages to an exchange:
    await _publishEndpoint.Publish(new OrderCreated
    {
        OrderId = order.Id,
        OrderPrice = order.TotalAmount,
        PaymentMethod = order.PaymentMethod
    });
    
5. Subscribing to Messages (Consumer):Implement IConsumer<T> in your subscriber service:
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly IPaymentService _paymentService;
    
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;
            var paymentDto = await _paymentService.GetPaymentStatusAsync(message.OrderId);
            if (paymentDto != null && paymentDto.PaymentStatus == PaymentStatus.Successful)
            {
                _logger.LogWarning("Payment is already completed");
                return;
            }
    
            await _paymentService.ProcessPaymentAsync(message.OrderId, message.OrderPrice, 
                message.PaymentMethod.MapPaymentMethod());
        }
    }
    Note: This consumer automatically subscribes to OrderCreated messages without extra configuration 
            because AddConsumers(Assembly.GetEntryAssembly()) registers it.
            
6. Ensure Both Services are Connected to RabbitMQ:
    Both publishers and subscribers need to register with RabbitMQ using AddRabbitMqWithMassTransit().
    
