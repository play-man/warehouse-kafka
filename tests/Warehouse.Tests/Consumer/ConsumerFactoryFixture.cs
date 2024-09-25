using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Warehouse.Consumer;
using Warehouse.Consumer.Extensions;
using Warehouse.Shared.Extensions;

namespace Warehouse.Tests.Consumer;

public class ConsumerFactoryFixture : WebApplicationFactory<Warehouse.Consumer.Program>, IAsyncLifetime
{
    private readonly KafkaContainerFixture _kafkaContainer;
    public ConsumerFactoryFixture(KafkaContainerFixture kafkaContainer)
    {
        _kafkaContainer = kafkaContainer;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IBusControl>();
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
