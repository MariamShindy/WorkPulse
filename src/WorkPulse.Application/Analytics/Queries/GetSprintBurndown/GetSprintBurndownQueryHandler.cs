using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetSprintBurndown;

public sealed class GetSprintBurndownQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetSprintBurndownQuery, Result<SprintBurndownDto>>
{
	public async Task<Result<SprintBurndownDto>> Handle(GetSprintBurndownQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string cacheKey = CacheKeys.SprintBurndown(tenantContext.TenantId, request.SprintId);
		SprintBurndownDto? cached = await cache.GetAsync<SprintBurndownDto>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		SprintBurndownDto? data = await analytics.GetSprintBurndownAsync(tenantContext.TenantId, request.SprintId, ct);
		if (data is null)
		{
			return Error.NotFound(SprintErrors.NotFoundCode, "Sprint not found.");
		}
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5L), ct);
		return Result.Success(data);
	}
}
