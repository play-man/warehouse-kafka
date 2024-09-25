using Serilog;
using Serilog.Events;

namespace Warehouse.Shared.Extensions;

public static class LoggingExtensions
{
    public static T ConfigureSerilog<T>(this T builder)
        where T : IHostBuilder
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.UseSerilog();

        return builder;
    }
}