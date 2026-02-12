using OrderTest.Api.Repositories;
using OrderTest.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

var app = builder.Build();

app.MapControllers();
app.Run();
