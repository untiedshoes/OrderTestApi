using OrderTest.Api.Models;
using System.Collections.Generic;
using System.Threading;

namespace OrderTest.Api.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private static List<Order> _orders = new();
    private static readonly object _lock = new();

    public Task AddAsync(Order order)
    {
        //ensure some level of thread safety
        lock (_lock)
        {
            _orders.Add(order);
            return Task.FromResult(order);
        }
    }

    public Task<Order?> GetByIdAsync(int id)
    {
        Order? result;
        //ensure some level of thread safety
        lock (_lock)
        {
            result = _orders.FirstOrDefault(o => o.Id == id);
        }
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Order>> GetAllAsync()
    {
        IEnumerable<Order> orders;

        //ensure some level of thread safety
        lock (_lock)
        {
            orders = _orders.ToList();
        }

        return Task.FromResult((IEnumerable<Order>) orders);
    }
}
