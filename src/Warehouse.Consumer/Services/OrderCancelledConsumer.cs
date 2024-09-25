using MassTransit;
using Warehouse.Models;

namespace Warehouse.Consumer.Services;

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(ApplicationDbContext dbContext, ILogger<OrderCancelledConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        _logger.LogInformation("Consuming OrderCancelledEvent: {@OrderCancelledEvent}", context.Message);

        var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = "Cancelled";
            order.CancelledAt = context.Message.CancelledAt;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order cancelled in database: {@Order}", order);
        }
        else
        {
            _logger.LogWarning("Order not found: {OrderId}", context.Message.OrderId);
        }
    }
}