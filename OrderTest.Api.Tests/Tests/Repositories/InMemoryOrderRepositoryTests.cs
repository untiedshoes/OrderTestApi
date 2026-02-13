using NUnit.Framework;
using OrderTest.Api.Models;
using OrderTest.Api.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace OrderTest.Api.Tests.Repositories;

[TestFixture]
public class InMemoryOrderRepositoryTests
{
    private readonly InMemoryOrderRepository _repository;

    public InMemoryOrderRepositoryTests()
    {
        _repository = new InMemoryOrderRepository();
    }

    [Test]
    public async Task AddAsync_ShouldAddOrder()
    {
        var order = new Order { 
            Id = 1,
            CustomerEmail = "craig@untiedshoes.co.uk"
        };

        await _repository.AddAsync(order);
        var result = await _repository.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CustomerEmail, Is.EqualTo(order.CustomerEmail));
    }

    [Test]
    public async Task AddAsync_ShouldOverwriteExistingOrder()
    {
        var order1 = new Order { 
            Id = 1,
            CustomerEmail = "craig@untiedshoes.co.uk"
        };

        var order2 = new Order { 
            Id = 1, 
            CustomerEmail = "craig.a.richards@untiedshoes.co.uk"
        };

        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);

        var result = await _repository.GetByIdAsync(1);
        Assert.That(result!.CustomerEmail, Is.EqualTo("craig.a.richards@untiedshoes.co.uk"));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var result = await _repository.GetByIdAsync(999);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        var order1 = new Order { 
            Id = 1, 
            CustomerEmail = "craig@untiedshoes.co.uk"
        };
        var order2 = new Order { 
            Id = 2, 
            CustomerEmail = "craig.a.richards@untiedshoes.co.uk"
        };

        await _repository.AddAsync(order1);
        await _repository.AddAsync(order2);

        var all = (await _repository.GetAllAsync()).ToList();

        Assert.That(all.Count, Is.EqualTo(2));
        Assert.That(all.Any(o => o.Id == 1), Is.True);
        Assert.That(all.Any(o => o.Id == 2), Is.True);
    }


    // As I swapped out the _Lock for a ConcurrentDictionary, that should ideally be tested too? Lets pump 100 orders through.

    [Test]
    public async Task AddAsync_ShouldHandleConcurrentAdds()
    {
        var tasks = new List<Task>();

        // 100 orders to add concurrently
        for (int i = 1; i <= 100; i++)
        {
            var orderId = i;
            tasks.Add(Task.Run(async () =>
            {
                var order = new Order { Id = orderId, CustomerEmail = $"user{orderId}@test.com" };
                await _repository.AddAsync(order);
            }));
        }

        await Task.WhenAll(tasks);

        var allOrders = (await _repository.GetAllAsync()).ToList();
        Assert.That(allOrders.Count, Is.EqualTo(100));
        for (int i = 1; i <= 100; i++)
        {
            Assert.That(allOrders.Any(o => o.Id == i), Is.True);
        }
    }
}
