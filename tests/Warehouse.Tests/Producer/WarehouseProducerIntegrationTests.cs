using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Warehouse.Controllers;

namespace Warehouse.Tests.Producer;

public class WarehouseProducerIntegrationTests : IClassFixture<KafkaContainerFixture>
{
    private readonly ProducerFactoryFixture _producerFactory;
    private readonly ILogger<WarehouseProducerIntegrationTests> _logger;
    private readonly HttpClient _client;
    
    public WarehouseProducerIntegrationTests(KafkaContainerFixture kafkaContainer)
    {
        _producerFactory = new ProducerFactoryFixture(kafkaContainer);
        _logger = _producerFactory.Services.GetRequiredService<ILogger<WarehouseProducerIntegrationTests>>();
        _client = _producerFactory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnSuccessResponse()
    {
        var createOrderDto = new CreateOrderDto
        {
            CustomerName = "John Doe",
            OrderAmount = 100
        };

        var response = await _client.PostAsJsonAsync("/api/purchaseorder/create", createOrderDto);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(responseBody);
        Assert.Equal("Order created", (string)responseBody.message);
    }

    [Fact]
    public async Task ChangeOrderAmount_ShouldThrow_WhenAmountIsNegative()
    {
        var changeOrderAmountDto = new ChangeOrderAmountDto
        {
            OrderId = Guid.NewGuid(),
            NewAmount = -50
        };
        
        var response = await _client.PutAsJsonAsync("/api/purchaseorder/change-amount", changeOrderAmountDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseBody = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.Equal("Order amount cannot be negative.", (string)responseBody.message);
    }

    [Fact]
    public async Task CancelOrder_ShouldReturnSuccessResponse()
    {
        var cancelOrderDto = new CancelOrderDto
        {
            OrderId = Guid.NewGuid()
        };

        var response = await _client.PutAsJsonAsync("/api/purchaseorder/cancel", cancelOrderDto);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(responseBody);
        Assert.Equal("Order cancelled", (string)responseBody.message);
    }
}