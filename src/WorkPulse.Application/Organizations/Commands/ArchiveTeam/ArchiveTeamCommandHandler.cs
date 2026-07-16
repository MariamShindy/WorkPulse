using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		Team team = await context.Teams.FirstOrDefaultAsync((Team t) => t.Id == request.TeamId, ct);
		if (team == null)
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		team.IsArchived = true;
		return Result.Success();
	}
}
