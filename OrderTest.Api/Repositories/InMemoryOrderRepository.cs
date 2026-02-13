using OrderTest.Api.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace OrderTest.Api.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private static ConcurrentDictionary<int, Order> _orders = new();

    public Task AddAsync(Order order)
    {
        _orders[order.Id] = order; // thread-safe without explicit lock
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        _orders.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Order>> GetAllAsync()
    {
        // Return a snapshot of all orders
        var snapshot = _orders.Values.ToList();
        return Task.FromResult((IEnumerable<Order>)snapshot);
    }
}
