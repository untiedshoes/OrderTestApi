using Microsoft.Extensions.Logging;
using Moq;
using OrderTest.Api.Models;
using OrderTest.Api.Repositories;
using OrderTest.Api.Services;

namespace OrderTest.Api.Tests;

[TestFixture]
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository>? _orderRepositoryMock;
    private readonly Mock<IEmailNotificationService>? _emailNotificationServiceMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _emailNotificationServiceMock = new Mock<IEmailNotificationService>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _emailNotificationServiceMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public void CreateOrderAsync_ShouldThrowArgumentNullException_WhenOrderIsNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _orderService.CreateOrderAsync(null));
    }

    [Test]
    public void CreateOrderAsync_ShouldThrowException_WhenEmailIsMissing()
    { 
        var order = new Order
        { 
            CustomerEmail = null 
        };
        
        var exception = Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrderAsync(order));
        
        Assert.That(exception.Message, Is.EqualTo("Customer email is required."));
    }

    [Test]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenEmailIsValid()
    {
        var order = new Order
        {
            CustomerEmail = "craig@untiedshoes.co.uk"
        };

        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _emailNotificationServiceMock
            .Setup(n => n.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _orderService.CreateOrderAsync(order);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));

        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);

        _emailNotificationServiceMock.Verify(n => n.SendEmailAsync(
            order.CustomerEmail,
            "Order created",
            "Your order has been created"),
            Times.Once);
    }

    [Test]
    public async Task CreateOrderAsync_ShouldStillReturnOrder_WhenEmailNotValid()
    {
        var order = new Order
        {
            CustomerEmail = "test@test.com"
        };

        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _emailNotificationServiceMock
            .Setup(n => n.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(new Exception("Email failed"));

        var result = await _orderService.CreateOrderAsync(order);

        Assert.That(result, Is.Not.Null);

        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
