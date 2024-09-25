using MassTransit;
using Warehouse.Models;

namespace Warehouse.Shared.Extensions;

public static class MassTransitProducerExtensions
{
    public static T AddMassTransitProducers<T>(this T services, Action<IRiderRegistrationContext, IKafkaFactoryConfigurator> configureKafkaProducers)
        where T : IServiceCollection
    {
        services.AddMassTransit(x =>
        {
            x.UsingInMemory();
            x.AddRider(r =>
            {
                r.AddProducer<OrderCreatedEvent>("order.created");
                r.AddProducer<OrderUpdatedEvent>("order.updated");
                r.AddProducer<OrderAmountChangedEvent>("order.amount.changed");
                r.AddProducer<OrderCancelledEvent>("order.cancelled");
                
                r.UsingKafka(configureKafkaProducers);
            });
        });
        return services;
    }
}