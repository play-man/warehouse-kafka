using MassTransit;
using StackExchange.Redis;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Shared;
using Warehouse.Shared.Extensions;
using Warehouse.Shared.Models;

namespace Warehouse.Producer;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging();
        builder.Services.AddControllers();
        builder.ConfigureOpenTelemetry();

        builder.Host.ConfigureKafkaOptions(); 
        builder.Host.ConfigureSerilog();
        builder.Services.AddMassTransitProducers((context, cfg) =>
        {
            cfg.Host(context);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseRouting();

        app.MapControllers();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=OrderUpdate}/{action=Index}/{id?}");

        app.Run();
    }
}

