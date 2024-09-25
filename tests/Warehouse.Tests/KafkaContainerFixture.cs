using Testcontainers.Kafka;

namespace Warehouse.Tests;

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Xunit;

public class KafkaContainerFixture : IAsyncLifetime
{
    public KafkaContainer KafkaContainer { get; private set; }
    public string BootstrapServers { get; private set; }

    public async Task InitializeAsync()
    {
        // Initialize Kafka container using TestContainers
        KafkaContainer = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:latest")
            .WithPortBinding(9092, true)
            .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "PLAINTEXT://localhost:9092")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9092))
            .Build();

        await KafkaContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await KafkaContainer.StopAsync();
    }
}
