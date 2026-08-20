using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetProjectProgress;

public sealed class GetProjectProgressQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetProjectProgressQuery, Result<IReadOnlyList<ProjectProgressDto>>>
{
	public async Task<Result<IReadOnlyList<ProjectProgressDto>>> Handle(GetProjectProgressQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string cacheKey = CacheKeys.ProjectProgress(tenantContext.TenantId, request.TeamId);
		IReadOnlyList<ProjectProgressDto>? cached = await cache.GetAsync<IReadOnlyList<ProjectProgressDto>>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		IReadOnlyList<ProjectProgressDto> data = await analytics.GetProjectProgressAsync(tenantContext.TenantId, request.TeamId, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(3L), ct);
		return Result.Success(data);
	}
}
