using MassTransit;
using Warehouse.Models;

namespace Warehouse.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<WarehouseService> _logger;

    public WarehouseService(IPublishEndpoint publishEndpoint, ILogger<WarehouseService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task CreateOrder(Guid orderId, string customerName, decimal amount)
    {
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            CreatedAt = DateTime.UtcNow,
            CustomerName = customerName,
            OrderAmount = amount
        };

        _logger.LogInformation("Publishing OrderCreatedEvent: {@OrderCreatedEvent}", orderCreatedEvent);
        await _publishEndpoint.Publish(orderCreatedEvent);
        _logger.LogInformation("OrderCreatedEvent published successfully.");
    }

    public async Task UpdateOrderStatus(Guid orderId, string status)
    {
        var orderUpdatedEvent = new OrderUpdatedEvent
        {
            OrderId = orderId,
            UpdatedAt = DateTime.UtcNow,
            Status = status
        };

        _logger.LogInformation("Publishing OrderUpdatedEvent: {@OrderUpdatedEvent}", orderUpdatedEvent);
        await _publishEndpoint.Publish(orderUpdatedEvent);
        _logger.LogInformation("OrderUpdatedEvent published successfully.");
    }

    public async Task ChangeOrderAmount(Guid orderId, decimal newAmount)
    {
        if (newAmount < 0)
        {
            throw new InvalidOperationException("Order amount cannot be negative.");
        }

        var orderAmountChangedEvent = new OrderAmountChangedEvent
        {
            OrderId = orderId,
            NewAmount = newAmount,
            UpdatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Publishing OrderAmountChangedEvent: {@OrderAmountChangedEvent}", orderAmountChangedEvent);
        await _publishEndpoint.Publish(orderAmountChangedEvent);
        _logger.LogInformation("OrderAmountChangedEvent published successfully.");
    }

    public async Task CancelOrder(Guid orderId)
    {
        var orderCancelledEvent = new OrderCancelledEvent
        {
            OrderId = orderId,
            CancelledAt = DateTime.UtcNow
        };

        _logger.LogInformation("Publishing OrderCancelledEvent: {@OrderCancelledEvent}", orderCancelledEvent);
        await _publishEndpoint.Publish(orderCancelledEvent);
        _logger.LogInformation("OrderCancelledEvent published successfully.");
    }
}

public interface IWarehouseService
{
    Task CreateOrder(Guid orderId, string customerName, decimal amount);
    Task UpdateOrderStatus(Guid orderId, string status);
    Task ChangeOrderAmount(Guid orderId, decimal newAmount);
    Task CancelOrder(Guid orderId);
}