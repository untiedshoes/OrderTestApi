using OrderTest.Api.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace OrderTest.Api.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<int, Order> _orders = new();

    public Task AddAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        _orders.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Order>> GetAllAsync()
    {
        return Task.FromResult((IEnumerable<Order>)_orders.Values);
    }
}
