using MassTransit;
using StackExchange.Redis;
using Warehouse.Consumer.Data;
using Warehouse.Models;
using Warehouse.Shared.Models;

namespace Warehouse.Consumer.Services;

public class OrderAmountChangedConsumer : IConsumer<OrderAmountChangedEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<OrderAmountChangedConsumer> _logger;

    public OrderAmountChangedConsumer(ApplicationDbContext dbContext, ILogger<OrderAmountChangedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderAmountChangedEvent> context)
    {
        _logger.LogInformation("Consuming OrderAmountChangedEvent: {@OrderAmountChangedEvent}", context.Message);

        var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.OrderAmount = context.Message.NewAmount;
            order.UpdatedAt = context.Message.UpdatedAt;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order amount updated in database: {@Order}", order);
        }
        else
        {
            _logger.LogWarning("Order not found: {OrderId}", context.Message.OrderId);
        }
    }
}
   
