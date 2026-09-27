using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetCycleTimeAnalytics;

public sealed class GetCycleTimeAnalyticsQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetCycleTimeAnalyticsQuery, Result<CycleTimeAnalyticsDto>>
{
	public async Task<Result<CycleTimeAnalyticsDto>> Handle(GetCycleTimeAnalyticsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string cacheKey = CacheKeys.CycleTimeAnalytics(tenantContext.TenantId, request.TeamId, request.From, request.To);
		CycleTimeAnalyticsDto? cached = await cache.GetAsync<CycleTimeAnalyticsDto>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		CycleTimeAnalyticsDto data = await analytics.GetCycleTimeAnalyticsAsync(tenantContext.TenantId, request.TeamId, request.From, request.To, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5L), ct);
		return Result.Success(data);
	}
}
