using System.Reflection;
using DbUp;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Warehouse.Tests;

public class RedisContainerFixture : IAsyncLifetime
{
    private readonly RedisContainer  _redisContainer;

    public RedisContainerFixture()
    {
        _redisContainer = new RedisContainer(new RedisConfiguration());
    }

    public string GetConnectionString()
    {
        return _redisContainer.GetConnectionString();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
    }


    public async Task DisposeAsync()
    {
        await _redisContainer.StopAsync();
    }
}

