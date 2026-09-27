using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Domain.Entities;

namespace WorkPulse.API.Hubs;

/// <summary>
/// Verifies that a hub caller may observe a given tenant, team or task.
/// <para>
/// Hub invocations bypass <c>TenantMembershipMiddleware</c> (it skips <c>/hubs</c>), so the hub
/// is the only place these checks can happen. Every query ignores the tenant query filter and
/// compares <c>TenantId</c> explicitly, so a stale or unset ambient tenant cannot widen access.
/// </para>
/// </summary>
public interface IHubTenantAuthorizer
{
	Task<bool> IsMemberOfTenantAsync(Guid userId, Guid tenantId, CancellationToken ct = default);

	Task<bool> CanAccessTeamAsync(Guid userId, Guid tenantId, Guid teamId, CancellationToken ct = default);

	Task<bool> CanAccessTaskAsync(Guid userId, Guid tenantId, Guid taskId, CancellationToken ct = default);
}

public sealed class HubTenantAuthorizer(IApplicationDbContext dbContext) : IHubTenantAuthorizer
{
	public Task<bool> IsMemberOfTenantAsync(Guid userId, Guid tenantId, CancellationToken ct = default)
	{
		return dbContext.CompanyMembers
			.IgnoreQueryFilters()
			.AsNoTracking()
			.AnyAsync(
				m => m.UserId == userId
				     && m.TenantId == tenantId
				     && m.IsActive
				     && !m.IsDeleted,
				ct);
	}

	public async Task<bool> CanAccessTeamAsync(Guid userId, Guid tenantId, Guid teamId, CancellationToken ct = default)
	{
		if (!await IsMemberOfTenantAsync(userId, tenantId, ct))
		{
			return false;
		}

		return await dbContext.Teams
			.IgnoreQueryFilters()
			.AsNoTracking()
			.AnyAsync((Team t) => t.Id == teamId && t.TenantId == tenantId && !t.IsDeleted, ct);
	}

	public async Task<bool> CanAccessTaskAsync(Guid userId, Guid tenantId, Guid taskId, CancellationToken ct = default)
	{
		if (!await IsMemberOfTenantAsync(userId, tenantId, ct))
		{
			return false;
		}

		return await dbContext.TaskItems
			.IgnoreQueryFilters()
			.AsNoTracking()
			.AnyAsync((TaskItem t) => t.Id == taskId && t.TenantId == tenantId && !t.IsDeleted, ct);
	}
}
