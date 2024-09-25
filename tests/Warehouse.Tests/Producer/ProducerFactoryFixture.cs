using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Warehouse.Consumer;
using Warehouse.Consumer.Extensions;
using Warehouse.Shared.Extensions;

namespace Warehouse.Tests.Producer;

public class ProducerFactoryFixture :  WebApplicationFactory<Warehouse.Producer.Program>, IAsyncLifetime
{
    private readonly KafkaContainerFixture _kafkaContainer;

    public ProducerFactoryFixture(KafkaContainerFixture kafkaContainer)
    {
        _kafkaContainer = kafkaContainer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBusControl>();
            services.RemoveAll<ApplicationDbContext>();
            services.AddMassTransitConsumers((context, cfg) =>
            {
                cfg.Host(context);
                cfg.ConfigureConsumers(context);
            });
        });
    }
    
    public async Task InitializeAsync()
    {
    }

    public async Task DisposeAsync()
    {
    }
}
