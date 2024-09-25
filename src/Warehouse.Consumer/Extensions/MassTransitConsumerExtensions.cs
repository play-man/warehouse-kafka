using MassTransit;
using Sample.Shared;
using Warehouse.Consumer.Services;
using Warehouse.Models;
using Warehouse.Shared.Extensions;

namespace Warehouse.Consumer.Extensions;

public static class MassTransitConsumerExtensions
{
    public static T AddMassTransitConsumers<T>(this T services, Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configureKafkaConsumers)
        where T : IServiceCollection
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();
            x.AddRider(r =>
            {
                r.AddConsumer<OrderCreatedConsumer>();
                r.AddConsumer<OrderUpdatedConsumer>();
                r.AddConsumer<OrderAmountChangedConsumer>();
                r.AddConsumer<OrderCancelledConsumer>();
                
                r.UsingKafka(configureKafkaConsumers);
            });
        });
        return services;
    }

    public static void ConfigureConsumers(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext context)
    {
        cfg.TopicEndpoint<string, OrderCreatedEvent>("order.created", "warehouse.consumers", e =>
        {
            e.UseConfiguration();
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
        cfg.TopicEndpoint<string, OrderUpdatedEvent>("order.updated", "warehouse.consumers", e =>
        {
            e.UseConfiguration();
            e.ConfigureConsumer<OrderUpdatedConsumer>(context);
        });
        cfg.TopicEndpoint<string, OrderAmountChangedEvent>("order.amount.changed", "warehouse.consumers",
            e =>
        {
            e.UseConfiguration();
            e.ConfigureConsumer<OrderAmountChangedConsumer>(context);
        });
        cfg.TopicEndpoint<string, OrderCancelledEvent>("order.cancelled", "warehouse.consumers", e =>
        {
            e.UseConfiguration();
            e.ConfigureConsumer<OrderCancelledConsumer>(context);
        });
    }
}