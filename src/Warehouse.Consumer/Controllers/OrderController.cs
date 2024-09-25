using Warehouse.Shared.Models;

namespace Warehouse.Consumer.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<OrderController> _logger;

    public OrderController(ApplicationDbContext dbContext, IDistributedCache cache, ILogger<OrderController> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
    }

    // Endpoint to retrieve order by ID with Redis caching
    [HttpGet("get/{orderId:guid}")]
    public async Task<IActionResult> GetOrderById(Guid orderId)
    {
        // Check if the order is in the cache
        var cachedOrder = await _cache.GetStringAsync(orderId.ToString());
        if (!string.IsNullOrEmpty(cachedOrder))
        {
            _logger.LogInformation("Returning cached order: {OrderId}", orderId);
            var orderFromCache = JsonSerializer.Deserialize<Order>(cachedOrder);
            return Ok(orderFromCache);
        }

        // If not found in cache, fetch from the database
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", orderId);
            return NotFound(new { Message = "Order not found", OrderId = orderId });
        }

        // Cache the order in Redis for future requests
        var serializedOrder = JsonSerializer.Serialize(order);
        await _cache.SetStringAsync(orderId.ToString(), serializedOrder, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Set cache TTL to 10 minutes
        });

        _logger.LogInformation("Returning order from database and caching: {OrderId}", orderId);
        return Ok(order);
    }
}
