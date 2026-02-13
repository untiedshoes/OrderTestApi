using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OrderTest.Api.Controllers;
using OrderTest.Api.Models;
using OrderTest.Api.Services;
using System.Threading.Tasks;

namespace OrderTest.Api.Tests.Controllers;

[TestFixture]
public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _controller = new OrdersController(_orderServiceMock.Object);
    }

    [Test]
    public async Task GetAsync_ShouldReturnOk_WhenOrderExists()
    {
        var order = new Order { 
            Id = 1,
            CustomerEmail = "craig@untiedshoes.co.uk" 
        };
        _orderServiceMock.Setup(s => s.GetOrderAsync(1)).ReturnsAsync(order);

        var result = await _controller.GetAsync(1);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value, Is.EqualTo(order));
        _orderServiceMock.Verify(s => s.GetOrderAsync(1), Times.Once);
    }

    [Test]
    public async Task GetAsync_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _orderServiceMock.Setup(s => s.GetOrderAsync(42)).ReturnsAsync((Order?)null);

        var result = await _controller.GetAsync(42);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
        _orderServiceMock.Verify(s => s.GetOrderAsync(42), Times.Once);
    }
}
