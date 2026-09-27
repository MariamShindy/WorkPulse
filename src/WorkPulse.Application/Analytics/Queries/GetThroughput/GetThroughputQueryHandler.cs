using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetThroughput;

public sealed class GetThroughputQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetThroughputQuery, Result<IReadOnlyList<ThroughputPointDto>>>
{
	public async Task<Result<IReadOnlyList<ThroughputPointDto>>> Handle(GetThroughputQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		int periods = Math.Clamp(request.Periods, 1, 52);
		string cacheKey = CacheKeys.Throughput(tenantContext.TenantId, request.TeamId, request.Granularity.ToString(), periods);
		IReadOnlyList<ThroughputPointDto>? cached = await cache.GetAsync<IReadOnlyList<ThroughputPointDto>>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		IReadOnlyList<ThroughputPointDto> data = await analytics.GetThroughputAsync(tenantContext.TenantId, request.TeamId, request.Granularity, periods, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5L), ct);
		return Result.Success(data);
	}
}
