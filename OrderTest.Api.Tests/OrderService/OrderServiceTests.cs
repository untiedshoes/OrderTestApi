using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OrderTest.Api.Controllers;
using OrderTest.Api.Models;
using OrderTest.Api.Repositories;
using OrderTest.Api.Services;
using System;
using System.Threading.Tasks;

namespace OrderTest.Api.Tests;

[TestFixture]
public class OrderTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IEmailNotificationService> _emailNotificationServiceMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly Mock<IOrderService> _orderServiceMock;

    private readonly OrderService _orderService;
    private readonly OrdersController _ordersController;

    public OrderTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _emailNotificationServiceMock = new Mock<IEmailNotificationService>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _orderServiceMock = new Mock<IOrderService>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _emailNotificationServiceMock.Object,
            _loggerMock.Object);

        _ordersController = new OrdersController(_orderServiceMock.Object);
    }

    // OrderService Tests

    [Test]
    public void CreateOrderAsync_ShouldThrowArgumentNullException_WhenOrderIsNull()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _orderService.CreateOrderAsync(null));
    }

    [Test]
    public void CreateOrderAsync_ShouldThrowException_WhenEmailIsMissing()
    {
        var order = new Order { 
            CustomerEmail = null 
        };

        var exception = Assert.ThrowsAsync<Exception>(() =>
            _orderService.CreateOrderAsync(order));

        Assert.That(exception.Message, Is.EqualTo("Customer email is required."));
    }

    [Test]
    public async Task CreateOrderAsync_ShouldCreateOrder_WhenEmailIsValid()
    {
        var order = new Order { 
            CustomerEmail = "craig@untiedshoes.co.uk" 
        };

        _orderRepositoryMock
            .Setup(o => o.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _emailNotificationServiceMock
            .Setup(e => e.SendEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _orderService.CreateOrderAsync(order);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.CreatedAt, Is.Not.EqualTo(default(DateTime)));

        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        _emailNotificationServiceMock.Verify(e =>
            e.SendEmailAsync(order.CustomerEmail, "Order created", "Your order has been created"),
            Times.Once);
    }

    [Test]
    public async Task CreateOrderAsync_ShouldStillReturnOrder_WhenEmailNotValid()
    {
        var order = new Order { 
            CustomerEmail = "craig@untiedshoes.co.uk" 
        };

        _orderRepositoryMock
            .Setup(o => o.AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);

        _emailNotificationServiceMock
            .Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Email failed"));

        var result = await _orderService.CreateOrderAsync(order);

        Assert.That(result, Is.Not.Null);
        _orderRepositoryMock.Verify(o => o.AddAsync(It.IsAny<Order>()), Times.AtLeastOnce);
    }

    [Test]
    public void CreateOrderAsync_ShouldThrowException_WhenRepositoryFails()
    {
        var order = new Order { 
            CustomerEmail = "craig@untiedshoes.co.uk" 
        };

        _orderRepositoryMock
            .Setup(o => o.AddAsync(It.IsAny<Order>()))
            .ThrowsAsync(new Exception("DB error"));

        Assert.ThrowsAsync<Exception>(() => _orderService.CreateOrderAsync(order));
    }

    // OrdersController Tests
  
    [Test]
    public async Task GetAsync_ShouldReturnOk_WhenOrderExists()
    {
        var order = new Order { 
            Id = 1, 
            CustomerEmail = "craig@untiedshoes.co.uk" 
        };

        _orderServiceMock.Setup(s => s.GetOrderAsync(1)).ReturnsAsync(order);

        var result = await _ordersController.GetAsync(1);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(order));
        _orderServiceMock.Verify(s => s.GetOrderAsync(1), Times.Once);
    }

    [Test]
    public async Task GetAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(s => s.GetOrderAsync(42)).ReturnsAsync((Order?)null);

        var result = await _ordersController.GetAsync(42);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        _orderServiceMock.Verify(s => s.GetOrderAsync(42), Times.Once);
    }
}
