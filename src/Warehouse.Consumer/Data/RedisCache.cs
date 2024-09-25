using StackExchange.Redis;

namespace Warehouse.Consumer.Data;

public interface IRedisCache
{
    Task<int> IncrementVideoCount(Guid videoId, int increment);
    Task<int> DecrementVideoCount(Guid videoId, int decrement);
}

public class RedisCache : IRedisCache
{
    private readonly IDatabase _redisDb;

    public RedisCache(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<int> IncrementVideoCount(Guid videoId, int increment)
    {
        var newCount = await _redisDb.StringIncrementAsync(videoId.ToString(), increment);
        return (int)newCount;
    }

    public async Task<int> DecrementVideoCount(Guid videoId, int decrement)
    {
        var newCount = await _redisDb.StringDecrementAsync(videoId.ToString(), decrement);
        return (int)newCount;
    }
}
