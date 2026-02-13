using OrderTest.Api.Models;
using OrderTest.Api.Repositories;

namespace OrderTest.Api.Services;

//make all methods async
public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailNotificationService _notificationService;
    private readonly ILogger _logger;

    public OrderService(
        IOrderRepository repository,
        IEmailNotificationService notificationService,
        ILogger<OrderService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task<Order?> CreateOrderAsync(Order order)
    {
        //make sure we test for null
        if (order is null) throw new ArgumentNullException(nameof(order));

        if (string.IsNullOrEmpty(order.CustomerEmail))
        {
            throw new Exception("Customer email is required.");
        }

        order.Id = new Random().Next(1, 1000);
        // Use a standardised time
        order.CreatedAt = DateTime.UtcNow;

        try
        {
            await _repository.AddAsync(order).ConfigureAwait(false);
            var createdOrder = order;

            // Send the email, but don't fail everything.
            try
            {
                await _notificationService.SendEmailAsync(
                    order.CustomerEmail,
                    "Order created",
                    "Your order has been created"
                ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //throw an error with the order id
                _logger.LogWarning(ex, "Error: Failed to send order-creation email for order {OrderId}", order.Id);
            }

            return createdOrder;
        }
        catch (Exception ex)
        {
            //throw an error with the customer email
            _logger.LogError(ex, "Error: Failed to create order for {Email}", order.CustomerEmail);
            throw;
        }
        
    }

    public Task<Order?> GetOrderAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Order>> GetOrdersAsync()
    {
        return _repository.GetAllAsync();
    }
}
