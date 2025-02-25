using CustomerService;
using CustomerService.Repositories;
using Microsoft.EntityFrameworkCore;
using DbContext = CustomerService.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);
IocConfiguration.ConfigureServices(builder.Services);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"), 
        new MySqlServerVersion(new Version(8,0,2))));

var app = builder.Build();


if ( app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer Service API v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.MapGet("/health", () => Results.Ok("Healthy"));
app.UseAuthorization();

app.MapControllers();

app.Run();