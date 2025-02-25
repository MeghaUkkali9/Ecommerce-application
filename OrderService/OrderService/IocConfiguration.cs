using FluentValidation;
using OrderService.Mappers;
using OrderService.Repositories;
using OrderService.Services;
using OrderService.Validators;

namespace OrderService;

public static class IocConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
        
        services.AddScoped<IOrderService, Services.OrderService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderMapper, OrderMapper>();
    }
}