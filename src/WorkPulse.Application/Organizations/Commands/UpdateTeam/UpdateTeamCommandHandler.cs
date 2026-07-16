using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeam;

public sealed class UpdateTeamCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateTeamCommand, Result<TeamDto>>
{
	public async Task<Result<TeamDto>> Handle(UpdateTeamCommand request, CancellationToken ct)
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
		team.Name = request.Name.Trim();
		team.Description = request.Description?.Trim();
		team.Icon = request.Icon;
		team.Color = request.Color;
		return new TeamDto(team.Id, team.Name, team.Key, team.Description, team.Icon, team.Color, team.IsArchived, team.CreatedAtUtc);
	}
}
