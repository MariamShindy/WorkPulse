using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkPulse.Application.Abstractions;

public interface ICacheService
{
	Task<T?> GetAsync<T>(string key, CancellationToken ct = default(CancellationToken));

	Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default(CancellationToken));

	Task RemoveAsync(string key, CancellationToken ct = default(CancellationToken));

	Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default(CancellationToken));
}
