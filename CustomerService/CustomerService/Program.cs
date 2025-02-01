using CustomerService;
using CustomerService.Repositories;
using Microsoft.EntityFrameworkCore;
using DbContext = CustomerService.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IocConfiguration.ConfigureServices(builder.Services);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"), 
        new MySqlServerVersion(new Version(8,0,2))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();