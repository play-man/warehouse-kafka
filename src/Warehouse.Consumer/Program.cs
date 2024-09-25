using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Warehouse.Consumer;
using Warehouse.Consumer.Data;
using Warehouse.Consumer.Extensions;
using Warehouse.Consumer.Services;
using Warehouse.Models;
using Warehouse.Shared;
using Warehouse.Shared.Extensions;
using Warehouse.Shared.Models;

namespace Warehouse.Consumer;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddLogging();
        builder.Services.AddControllers();
        builder.ConfigureOpenTelemetry();
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration["ConnectionStrings:PostgreSql"]));

        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(builder.Configuration["Redis:Host"]));
        builder.Services.AddSingleton<IRedisCache, RedisCache>();

        builder.Host.ConfigureKafkaOptions();
        builder.Services.AddMassTransitConsumers((context, cfg) =>
        {
            cfg.Host(context);
            cfg.ConfigureConsumers(context);
        });

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Order}/{action=Index}/{id?}");

        app.Run(); 

    }
}
