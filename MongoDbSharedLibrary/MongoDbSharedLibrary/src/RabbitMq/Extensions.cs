using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbSharedLibrary.Settings;

namespace MongoDbSharedLibrary.RabbitMq;

public static class Extensions
{
    public static IServiceCollection AddRabbitMqWithMassTransit(this IServiceCollection service)
    {
        service.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());
            
            configure.UsingRabbitMq((context, configurator) =>
                {
                    var configuration = context.GetService<IConfiguration>();
                    var serviceSetting = configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
                    var rabbitMqSetting = configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>();
                    
                    configurator.Host(rabbitMqSetting.Host);
                    configurator.ConfigureEndpoints(context,
                        new KebabCaseEndpointNameFormatter(serviceSetting.ServiceName, false));
                }
            );
        });
       
        return service;
    }
}