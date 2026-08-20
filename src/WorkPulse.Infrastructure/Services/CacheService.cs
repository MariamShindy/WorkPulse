using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WorkPulse.Infrastructure.Services;

public sealed class CacheService(IDistributedCache cache) : ICacheService
{
	private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default(CancellationToken))
	{
		byte[]? bytes = await cache.GetAsync(key, ct);
		if (bytes == null)
		{
			return default;
		}
		return JsonSerializer.Deserialize<T>(bytes, Options);
	}

	public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default(CancellationToken))
	{
		DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = (expiry ?? TimeSpan.FromMinutes(5L))
		};
		byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value, Options);
		await cache.SetAsync(key, bytes, options, ct);
	}

	public Task RemoveAsync(string key, CancellationToken ct = default(CancellationToken))
	{
		return cache.RemoveAsync(key, ct);
	}

	public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default(CancellationToken))
	{
		throw new NotSupportedException("Prefix removal requires direct Redis access. Use IConnectionMultiplexer.");
	}
}
