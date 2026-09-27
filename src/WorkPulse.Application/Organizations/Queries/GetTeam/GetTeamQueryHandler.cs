using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.GetTeam;

public sealed class GetTeamQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetTeamQuery, Result<TeamDto>>
{
	public async Task<Result<TeamDto>> Handle(GetTeamQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Team? team = await context.Teams.AsNoTracking().FirstOrDefaultAsync((Team t) => t.Id == request.TeamId, ct);
		if (team is null)
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		return new TeamDto(team.Id, team.Name, team.Key, team.Description, team.Icon, team.Color, team.IsArchived, team.CreatedAtUtc);
	}
}
