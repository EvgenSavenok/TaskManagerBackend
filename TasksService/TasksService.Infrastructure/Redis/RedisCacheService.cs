using System.Text.Json;
using Application.Contracts.Redis;
using StackExchange.Redis;

namespace TasksService.Infrastructure.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _cache;
    
    public RedisCacheService(string connectionString)
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
        _cache = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _cache.StringSetAsync(key, jsonData, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var jsonData = await _cache.StringGetAsync(key);
        
        return jsonData.HasValue ? JsonSerializer.Deserialize<T>(jsonData!) : default;
    }
}