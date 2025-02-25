using PaymentService.Mappers;
using PaymentService.Repositories;
using PaymentService.Services;

namespace PaymentService;

public static class IocConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentService, Services.PaymentService>();
        services.AddScoped<IPaymentMapper, PaymentMapper>();
    }
}