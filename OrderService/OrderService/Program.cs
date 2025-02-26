using Microsoft.EntityFrameworkCore;
using MongoDbSharedLibrary.RabbitMq;
using OrderService;
using OrderService.Data;
using OrderService.Proxies;

var builder = WebApplication.CreateBuilder(args);

IocConfiguration.ConfigureServices(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
        new MySqlServerVersion(new Version(8, 0, 2))));

builder.Services.AddHttpClient<ICustomerProxy, CustomerProxy>(client => {
    var apiUrl = builder.Configuration["CustomerService:Url"];
    client.BaseAddress = new Uri(apiUrl); });
builder.Services.AddHttpClient<IProductProxy, ProductProxy>(client => {
    var apiUrl = builder.Configuration["ProductService:Url"];
    client.BaseAddress = new Uri(apiUrl); });

builder.Services.AddRabbitMqWithMassTransit();
// builder.Services.AddMassTransit(configure =>
// {
//     configure.AddConsumers(Assembly.GetEntryAssembly());
//             
//     configure.UsingRabbitMq((context, configurator) =>
//         {
//             var serviceSetting = builder.Configuration.GetSection(nameof(ServiceSetting)).Get<ServiceSetting>();
//             var rabbitMqSetting = builder.Configuration.GetSection(nameof(RabbitMqSetting)).Get<RabbitMqSetting>();
//                     
//             configurator.Host(rabbitMqSetting.Host);
//             configurator.ConfigureEndpoints(context,
//                 new KebabCaseEndpointNameFormatter(serviceSetting.ServiceName, false));
//         }
//     );
// });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();