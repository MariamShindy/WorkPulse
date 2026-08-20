using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListTeamMembers;

public sealed class ListTeamMembersQueryHandler(
	IApplicationDbContext context,
	ITenantContext tenantContext,
	IUserIdentityService userIdentity) : IRequestHandler<ListTeamMembersQuery, Result<PagedList<TeamMemberDto>>>
{
	public async Task<Result<PagedList<TeamMemberDto>>> Handle(ListTeamMembersQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		if (!(await context.Teams.AnyAsync((Team t) => t.Id == request.TeamId, ct)))
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}

		IQueryable<TeamMember> query = context.TeamMembers
			.AsNoTracking()
			.ForTenant(tenantContext)
			.Where((TeamMember m) => m.TeamId == request.TeamId)
			.OrderBy((TeamMember m) => m.CreatedAtUtc);

		int totalCount = await query.CountAsync(ct);
		List<TeamMember> members = await query
			.Skip(request.Pagination.Skip)
			.Take(request.Pagination.PageSize)
			.ToListAsync(ct);

		IReadOnlyDictionary<Guid, UserIdentityDto> users = await userIdentity.GetByIdsAsync(
			members.Select((TeamMember m) => m.UserId),
			ct);

		List<TeamMemberDto> items = members.Select((TeamMember m) =>
		{
			users.TryGetValue(m.UserId, out UserIdentityDto? user);
			string fullName = user == null
				? string.Empty
				: (user.FirstName + " " + user.LastName).Trim();
			return new TeamMemberDto(
				m.Id,
				m.TeamId,
				m.UserId,
				user?.Email ?? string.Empty,
				fullName,
				m.Role.ToString(),
				m.CreatedAtUtc);
		}).ToList();

		return new PagedList<TeamMemberDto>(items, request.Pagination.Page, request.Pagination.PageSize, totalCount);
	}
}
