using CustomerService.Mappers;
using CustomerService.Repositories;
using CustomerService.Services;

namespace CustomerService;

public static class IocConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerService, Services.CustomerService>();
        services.AddScoped<ICustomerMapper, CustomerMapper>();
        
        services.AddScoped<IPaymentDetailsRepository, PaymentDetailsRepository>();
        services.AddScoped<IPaymentDetailsMapper, PaymentDetailsMapper>();
    }
}