using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetDashboardAnalytics;

public sealed class GetDashboardAnalyticsQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetDashboardAnalyticsQuery, Result<DashboardAnalyticsDto>>
{
	public async Task<Result<DashboardAnalyticsDto>> Handle(GetDashboardAnalyticsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string cacheKey = CacheKeys.Dashboard(tenantContext.TenantId, request.TeamId, request.From, request.To);
		DashboardAnalyticsDto? cached = await cache.GetAsync<DashboardAnalyticsDto>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		DashboardAnalyticsDto data = await analytics.GetDashboardAsync(tenantContext.TenantId, request.TeamId, request.From, request.To, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(2L), ct);
		return Result.Success(data);
	}
}
