using System.Collections;
using OrderTest.Api.Models;

namespace OrderTest.Api.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
}
