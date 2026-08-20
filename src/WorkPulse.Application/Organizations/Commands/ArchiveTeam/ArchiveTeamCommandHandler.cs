
namespace WorkPulse.Application.Organizations.Commands.ArchiveTeam;

public sealed class ArchiveTeamCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ArchiveTeamCommand, Result>
{
	public async Task<Result> Handle(ArchiveTeamCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Team? team = await context.Teams.FirstOrDefaultAsync((Team t) => t.Id == request.TeamId, ct);
		if (team is null)
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		team.IsArchived = true;
		return Result.Success();
	}
}
