using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetTeamVelocity;

public sealed class GetTeamVelocityQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetTeamVelocityQuery, Result<IReadOnlyList<TeamVelocityPointDto>>>
{
	public async Task<Result<IReadOnlyList<TeamVelocityPointDto>>> Handle(GetTeamVelocityQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		int weeks = Math.Clamp(request.Weeks, 1, 52);
		string cacheKey = CacheKeys.TeamVelocity(tenantContext.TenantId, request.TeamId, weeks);
		IReadOnlyList<TeamVelocityPointDto>? cached = await cache.GetAsync<IReadOnlyList<TeamVelocityPointDto>>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		IReadOnlyList<TeamVelocityPointDto> data = await analytics.GetTeamVelocityAsync(tenantContext.TenantId, request.TeamId, weeks, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5L), ct);
		return Result.Success(data);
	}
}
