using MassTransit;
using Warehouse.Models;
using Warehouse.Shared.Models;

namespace Warehouse.Consumer.Services;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ApplicationDbContext dbContext, ILogger<OrderCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        _logger.LogInformation("Consuming OrderCreatedEvent: {@OrderCreatedEvent}", context.Message);

        var order = new Order
        {
            OrderId = context.Message.OrderId,
            CustomerName = context.Message.CustomerName,
            CreatedAt = context.Message.CreatedAt,
            OrderAmount = context.Message.OrderAmount,
            Status = "Created"
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order created in database: {@Order}", order);
    }
}