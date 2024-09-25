using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Shared;

namespace Warehouse.Shared.Extensions;

public static class KafkaConfigurationExtensions
{
    /// <summary>
    /// Configure the Confluent Schema Registry for connection to Confluent Cloud
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ConfigureKafkaOptions<T>(this T builder)
        where T : IHostBuilder
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<KafkaOptions>()
                .BindConfiguration("Kafka");
        });
        return builder;
    }

    /// <summary>
    /// Just an example, but some retry/kill switch combination to stop processing when the consumer/saga faults repeatedly.
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseSampleRetryConfiguration(this IKafkaTopicReceiveEndpointConfigurator configurator)
    {
        configurator.UseKillSwitch(k => k.SetActivationThreshold(1).SetRestartTimeout(m: 1).SetTripThreshold(0.2).SetTrackingPeriod(m: 1));
        configurator.UseMessageRetry(retry => retry.Interval(1000, TimeSpan.FromSeconds(1)));
    }
    
    /// <summary>
    /// Setup limits and offsets.
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseLimitConfiguration(this IKafkaTopicReceiveEndpointConfigurator configurator)
    {
        configurator.Offset = 0;
        configurator.AutoOffsetReset = AutoOffsetReset.Earliest;
        configurator.ConcurrentMessageLimit = 10;
        configurator.ConcurrentConsumerLimit = 2;
        configurator.ConcurrentDeliveryLimit = 1;
    }
    
    /// <summary>
    /// Just an example, but some retry/kill switch combination to stop processing when the consumer/saga faults repeatedly.
    /// </summary>
    /// <param name="configurator"></param>
    public static void UseConfiguration(this IKafkaTopicReceiveEndpointConfigurator configurator)
    {
        configurator.UseLimitConfiguration();
        configurator.UseSampleRetryConfiguration();
    }

    /// <summary>
    /// Configure the Kafka Host using SASL_SSL to connect to Confluent Cloud
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="context"></param>
    public static void Host(this IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
    {
        var options = context.GetRequiredService<IOptions<KafkaOptions>>().Value;

        configurator.Host(options.Servers, h =>
        {
            // h.UseSasl(s =>
            // {
            //     s.SecurityProtocol = SecurityProtocol.SaslSsl;
            //     s.Mechanism = SaslMechanism.Plain;
            //     s.Username = options.Username;
            //     s.Password = options.Password;
            // });
        });
    }
}