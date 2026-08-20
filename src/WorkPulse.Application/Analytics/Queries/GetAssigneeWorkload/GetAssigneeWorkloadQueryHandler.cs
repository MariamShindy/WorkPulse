using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Common;

namespace WorkPulse.Application.Analytics.Queries.GetAssigneeWorkload;

public sealed class GetAssigneeWorkloadQueryHandler(IAnalyticsReadService analytics, ICacheService cache, ITenantContext tenantContext) : IRequestHandler<GetAssigneeWorkloadQuery, Result<IReadOnlyList<AssigneeWorkloadDto>>>
{
	public async Task<Result<IReadOnlyList<AssigneeWorkloadDto>>> Handle(GetAssigneeWorkloadQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string cacheKey = CacheKeys.AssigneeWorkload(tenantContext.TenantId, request.TeamId);
		IReadOnlyList<AssigneeWorkloadDto>? cached = await cache.GetAsync<IReadOnlyList<AssigneeWorkloadDto>>(cacheKey, ct);
		if (cached is not null)
		{
			return Result.Success(cached);
		}
		IReadOnlyList<AssigneeWorkloadDto> data = await analytics.GetAssigneeWorkloadAsync(tenantContext.TenantId, request.TeamId, ct);
		await cache.SetAsync(cacheKey, data, TimeSpan.FromMinutes(3L), ct);
		return Result.Success(data);
	}
}
