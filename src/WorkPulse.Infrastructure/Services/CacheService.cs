using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class CacheService(IDistributedCache cache) : ICacheService
{
	private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default(CancellationToken))
	{
		byte[] bytes = await cache.GetAsync(key, ct);
		return (T?)((bytes == null) ? ((object)default(T)) : ((object)JsonSerializer.Deserialize<T>(bytes, Options)));
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
