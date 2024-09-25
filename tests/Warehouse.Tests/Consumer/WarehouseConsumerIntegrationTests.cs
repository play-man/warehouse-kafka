using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Warehouse.Consumer;
using Warehouse.Tests.Producer;
using Warehouse.Consumer;

namespace Warehouse.Tests.Consumer;

using Order = Shared.Models.Order;

public class WarehouseConsumerIntegrationTests : IClassFixture<DbContainerFixture>, IClassFixture<KafkaContainerFixture>, IClassFixture<RedisContainerFixture>
{
    private readonly ConsumerFactoryFixture _consumerFactory;
    private readonly ILogger<WarehouseConsumerIntegrationTests> _logger;
    private readonly DbContainerFixture _dbContainer;
    private readonly IConnectionMultiplexer _redis;
    private readonly HttpClient _client;

    public WarehouseConsumerIntegrationTests(KafkaContainerFixture kafkaContainer, DbContainerFixture dbContainer, RedisContainerFixture redisContainer)
    {
        _consumerFactory = new ConsumerFactoryFixture(kafkaContainer);
        _logger = _consumerFactory.Services.GetRequiredService<ILogger<WarehouseConsumerIntegrationTests>>();
        _dbContainer = dbContainer;
        _redis = ConnectionMultiplexer.Connect(redisContainer.GetConnectionString());
        _client = _consumerFactory.CreateClient();
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrderAndCacheInRedis()
    {
        var orderId = Guid.NewGuid();

        using var scope = _consumerFactory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Orders.Add(new Order
        {
            OrderId = orderId,
            CustomerName = "John Doe",
            OrderAmount = 100,
            CreatedAt = DateTime.UtcNow,
            Status = "Created"
        });
        await dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/order/get/{orderId}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<Order>(responseBody);

        Assert.NotNull(order);
        Assert.Equal("John Doe", order.CustomerName);

        var redisDb = _redis.GetDatabase();
        var cachedOrder = await redisDb.StringGetAsync(orderId.ToString());
        Assert.False(cachedOrder.IsNullOrEmpty);
        var cachedOrderObj = JsonConvert.DeserializeObject<Order>(cachedOrder);

        Assert.Equal("John Doe", cachedOrderObj.CustomerName);

        var response2 = await _client.GetAsync($"/api/order/get/{orderId}");
        response2.EnsureSuccessStatusCode();
        var responseBody2 = await response2.Content.ReadAsStringAsync();
        var orderFromCache = JsonConvert.DeserializeObject<Order>(responseBody2);

        Assert.NotNull(orderFromCache);
        Assert.Equal("John Doe", orderFromCache.CustomerName);
    }
}
