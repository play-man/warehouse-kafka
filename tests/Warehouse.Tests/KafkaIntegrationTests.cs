using MassTransit;
using Microsoft.EntityFrameworkCore;
using Warehouse.Consumer;
using Warehouse.Consumer.Services;

using Confluent.Kafka;
using Testcontainers.Kafka;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Warehouse.Shared.Extensions;

namespace Warehouse.Tests;

// public class KafkaIntegrationTests : IAsyncLifetime
// {
//     private KafkaContainer _kafkaContainer;
//     private IHost _consumerHost;
//     private IServiceScopeFactory _serviceScopeFactory;
//
//     public KafkaIntegrationTests()
//     {
//         _kafkaContainer = new KafkaBuilder()
//             .WithPortBinding(9092, true)
//             .WithReuse(true)
//             .Build();
//     }
//
//     public async Task InitializeAsync()
//     {
//         // Start Kafka container
//         await _kafkaContainer.StartAsync();
//
//         // Configure and start your consumer service host
//         _consumerHost = Host.CreateDefaultBuilder()
//             .ConfigureServices((context, services) =>
//             {
//                 services.AddDbContext<ApplicationDbContext>(options =>
//                     options.UseNpgsql("Host=localhost;Database=testdb;Username=postgres;Password=password"));
//             })
//             .ConfigureMassTransit(())
//             .Build();_kafkaTestFixture.BootstrapServers
//
//         _serviceScopeFactory = _consumerHost.Services.GetService<IServiceScopeFactory>();
//
//         await _consumerHost.StartAsync();
//     }
//
//     public async Task DisposeAsync()
//     {
//         await _consumerHost.StopAsync();
//         await _kafkaContainer.StopAsync();
//     }
//
//     [Fact]
//     public async Task KafkaConsumer_ShouldProcessOrderCreatedEvent()
//     {
//         var orderId = Guid.NewGuid();
//         var orderCreatedEvent = new
//         {
//             OrderId = orderId,
//             CustomerName = "John Doe",
//             OrderAmount = 100,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         var config = new ProducerConfig { BootstrapServers = _kafkaContainer.GetBootstrapAddress() };
//         using var producer = new ProducerBuilder<Null, string>(config).Build();
//         var message = new Message<Null, string>
//         {
//             Value = JsonSerializer.Serialize(orderCreatedEvent)
//         };
//
//         // Produce the message to Kafka topic
//         await producer.ProduceAsync("order-created-topic", message);
//
//         // Verify that the order was saved in the database
//         using var scope = _serviceScopeFactory.CreateScope();
//         var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//         var order = await dbContext.Orders.FindAsync(orderId);
//
//         Assert.NotNull(order);
//         Assert.Equal("John Doe", order.CustomerName);
//         Assert.Equal(100, order.OrderAmount);
//     }
// }
