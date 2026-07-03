using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, Options);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, Options);
        await cache.SetAsync(key, bytes, options, ct);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default) =>
        cache.RemoveAsync(key, ct);

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // Redis-backed cache key prefix removal requires a Redis-specific implementation.
        // This is a no-op placeholder; wire StackExchange.Redis directly when needed.
        throw new NotSupportedException("Prefix removal requires direct Redis access. Use IConnectionMultiplexer.");
    }
}
