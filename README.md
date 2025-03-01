## To work with RabbitMQ:
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

## Docker
This project uses Docker Compose to run MySQL, RabbitMQ, and MongoDB as containers. MySQL is used for storing relational data, RabbitMQ handles messaging between services, and MongoDB is used for NoSQL storage. Volumes are added to keep data safe even if the containers restart.
```
services:
  mysql:
    image: mysql:8.0
    container_name: mysql
    restart: always
    ports:
      - "3306:3306"
    environment:
      MYSQL_ROOT_PASSWORD: <user_password>
    volumes:
      - mysql_data:/var/lib/mysql
      - ./init-scripts:/docker-entrypoint-initdb.d 

  rabbitmq:
    image: rabbitmq:management
    container_name: rabbitmq
    ports:
      - 5672:5672
      - 15672:15672
    volumes:
      - rabbitmqdata:/var/lib/rabbitmq
    hostname: rabbitmq
  
  mongo:
    image: mongo
    container_name: mongo
    ports:
      - "27017:27017"
    volumes:
      - mongodata:/data/db
        
volumes:
  mysql_data:
  rabbitmqdata:
  mongodata:

```


## Publishing locally and using the NuGet package in our project without pushing it 
to a remote NuGet repository.

Steps to Locally Publish and Use a NuGet Package

1. Pack the Library as a NuGet Package
    Run the following command in the MongoDbSharedLibrary project folder from where you see .csproj file:
        => **********-MacBook-Air MongoDbSharedLibrary % 
        => dotnet pack --output ../../local-nuget-package
        (This will create a .nupkg file inside the local-nuget-package directory.)
2. Add the Local NuGet Source
    In the project where you want to use the library, run:
        => dotnet nuget add source ./local-nuget-package --name LocalNuGet
        (This tells .NET to look for NuGet packages in the local-nuget-package directory.)
3. Install the NuGet Package Locally
    In the project where you want to use it, add the package:
        => dotnet add package MongoDbSharedLibrary --source LocalNuGet
         dotnet add package MongoDbSharedLibrary --source ../../local-nuget-package --version 1.0.1
4. Verify Installation
    You can check if the package is installed by running:
        => dotnet list package
        
Now, you can use your MongoDbSharedLibrary like any other NuGet package without publishing it to NuGet.org.


    
