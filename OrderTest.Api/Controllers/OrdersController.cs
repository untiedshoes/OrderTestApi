using Microsoft.AspNetCore.Mvc;
using OrderTest.Api.Models;
using OrderTest.Api.Services;

namespace OrderTest.Api.Controllers;

//make all methods async

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(Order order)
    {
        var createdOrder = await _orderService.CreateOrderAsync(order);
        return Ok(createdOrder);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var order = await _orderService.GetOrderAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}
