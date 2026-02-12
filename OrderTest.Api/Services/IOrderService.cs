using OrderTest.Api.Models;
using System.Collections;

namespace OrderTest.Api.Services;

public interface IOrderService
{
    Task<Order?> CreateOrderAsync(Order order);
    Task<Order?> GetOrderAsync(int id);
    Task<IEnumerable<Order>> GetOrdersAsync();
}
