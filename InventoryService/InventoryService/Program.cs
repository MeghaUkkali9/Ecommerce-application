using InventoryService.Data;
using InventoryService.Proxies;
using InventoryService.Services;
using MongoDbSharedLibrary.MongoDB;
using MongoDbSharedLibrary.RabbitMq;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IInventoryService,InventoryService.Services.InventoryService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMongo()
    .AddMongoRepository<Inventory>("Inventory")
    .AddRabbitMqWithMassTransit();

var jiterer = new Random();
builder.Services.AddHttpClient<IProductProxy, ProductProxy>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5282/");
    })
    .AddTransientHttpErrorPolicy(builder =>
        builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                            + TimeSpan.FromMilliseconds(jiterer.Next(1, 1000))
        ))
    .AddTransientHttpErrorPolicy(builder =>
        builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            5,
            TimeSpan.FromSeconds(30)))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));


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