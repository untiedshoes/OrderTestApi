using NUnit.Framework;
using OrderTest.Api.Models;
using OrderTest.Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderTest.Api.Tests.Repositories;

[TestFixture]
public class InMemoryOrderRepositoryTests
{
    [Test]
    public async Task AddAsync_ShouldAddOrder()
    {
        var repository = new InMemoryOrderRepository();

        var order = new Order { Id = 1, CustomerEmail = "craig@untiedshoes.co.uk" };

        await repository.AddAsync(order);
        var result = await repository.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CustomerEmail, Is.EqualTo(order.CustomerEmail));
    }

    [Test]
    public async Task AddAsync_ShouldOverwriteExistingOrder()
    {
        var repository = new InMemoryOrderRepository();

        var order1 = new Order { Id = 1, CustomerEmail = "craig@untiedshoes.co.uk" };
        var order2 = new Order { Id = 1, CustomerEmail = "craig.a.richards@untiedshoes.co.uk" };

        await repository.AddAsync(order1);
        await repository.AddAsync(order2);

        var result = await repository.GetByIdAsync(1);
        Assert.That(result!.CustomerEmail, Is.EqualTo("craig.a.richards@untiedshoes.co.uk"));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var repository = new InMemoryOrderRepository();

        var result = await repository.GetByIdAsync(999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        var repository = new InMemoryOrderRepository();

        var order1 = new Order { Id = 1, CustomerEmail = "craig@untiedshoes.co.uk" };
        var order2 = new Order { Id = 2, CustomerEmail = "craig.a.richards@untiedshoes.co.uk" };

        await repository.AddAsync(order1);
        await repository.AddAsync(order2);

        var all = (await repository.GetAllAsync()).ToList();

        Assert.That(all.Count, Is.EqualTo(2));
        Assert.That(all.Any(o => o.Id == 1), Is.True);
        Assert.That(all.Any(o => o.Id == 2), Is.True);
    }


    // As I swapped out the _Lock for a ConcurrentDictionary, that should ideally be tested too? Lets pump 100 orders through.

    [Test]
    public async Task AddAsync_ShouldHandleConcurrentAdds()
    {
        var repository = new InMemoryOrderRepository();
        var tasks = new List<Task>();

        for (int i = 1; i <= 100; i++)
        {
            var orderId = i;
            tasks.Add(Task.Run(async () =>
            {
                var order = new Order
                {
                    Id = orderId,
                    CustomerEmail = $"user{orderId}@test.com"
                };

                await repository.AddAsync(order);
            }));
        }

        await Task.WhenAll(tasks);

        var allOrders = (await repository.GetAllAsync()).ToList();

        Assert.That(allOrders.Count, Is.EqualTo(100));
        for (int i = 1; i <= 100; i++)
        {
            Assert.That(allOrders.Any(o => o.Id == i), Is.True);
        }
    }


    // Concurrent Overwrite Test

    [Test]
    public async Task AddAsync_ShouldHandleConcurrentOverwrites()
    {
        var repository = new InMemoryOrderRepository();
        var tasks = new List<Task>();

        // 50 threads writing to the same ID
        for (int i = 1; i <= 50; i++)
        {
            var email = $"user{i}@test.com";
            tasks.Add(Task.Run(async () =>
            {
                var order = new Order
                {
                    Id = 1,
                    CustomerEmail = email
                };

                await repository.AddAsync(order);
            }));
        }

        await Task.WhenAll(tasks);

        var result = await repository.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(1));

        // Only one order should exist
        var all = (await repository.GetAllAsync()).ToList();
        Assert.That(all.Count, Is.EqualTo(1));
    }
}