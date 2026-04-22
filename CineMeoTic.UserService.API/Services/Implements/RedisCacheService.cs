using CineMeoTic.UserService.API.Services.Intefaces;
using Serilog;
using StackExchange.Redis;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class RedisCacheService(IConnectionMultiplexer redisConnection) : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redisConnection = redisConnection;

    public IDatabase RedisDatabase => GetDatabase();

    public IDatabase GetDatabase()
    {
        return _redisConnection.GetDatabase();
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null)
    {
        try
        {
            IDatabase redisDatabase = GetDatabase();
            await redisDatabase.StringSetAsync(key, value, expiry, when: When.Always);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to set string in Redis cache for key: {Key}", key);
        }
    }

    public async Task<string?> GetStringAsync(string key)
    {
        IDatabase redisDatabase = GetDatabase();
        return await redisDatabase.StringGetAsync(key);
    }
}
