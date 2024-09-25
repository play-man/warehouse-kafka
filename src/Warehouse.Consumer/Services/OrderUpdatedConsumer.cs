using MassTransit;
using Warehouse.Models;

namespace Warehouse.Consumer.Services;

public class OrderUpdatedConsumer : IConsumer<OrderUpdatedEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderUpdatedConsumer> _logger;

    public OrderUpdatedConsumer(ApplicationDbContext dbContext, ILogger<OrderUpdatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderUpdatedEvent> context)
    {
        _logger.LogInformation("Consuming OrderUpdatedEvent: {@OrderUpdatedEvent}", context.Message);

        var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = context.Message.Status;
            order.UpdatedAt = context.Message.UpdatedAt;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order status updated in database: {@Order}", order);
        }
        else
        {
            _logger.LogWarning("Order not found: {OrderId}", context.Message.OrderId);
        }
    }
}