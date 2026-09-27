using WorkPulse.Application.Common;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Application.Organizations.Services;

namespace WorkPulse.Application.Organizations.Commands.CreateTeam;

public sealed class CreateTeamCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<CreateTeamCommand, Result<TeamDto>>
{
	public async Task<Result<TeamDto>> Handle(CreateTeamCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		string key = (string.IsNullOrWhiteSpace(request.Key) ? SlugHelper.ToKey(request.Name) : request.Key.Trim().ToUpperInvariant());
		if (await context.Teams.AsNoTracking().AnyAsync((Team t) => t.TenantId == tenantContext.TenantId && t.Key == key, ct))
		{
			return Error.Conflict("Team.KeyTaken", "Team key '" + key + "' is already in use.");
		}
		Team team = new Team
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			Name = request.Name.Trim(),
			Key = key,
			Description = request.Description?.Trim(),
			Icon = request.Icon,
			Color = request.Color
		};
		context.Teams.Add(team);
		context.TeamIssueCounters.Add(new TeamIssueCounter
		{
			TeamId = team.Id,
			TenantId = tenantContext.TenantId,
			LastNumber = 0
		});
		Workflow workflow;
		IReadOnlyList<WorkflowState> states;
		(workflow, states) = DefaultWorkflowFactory.Create(tenantContext.TenantId, team.Id);
		context.Workflows.Add(workflow);
		context.WorkflowStates.AddRange(states);
		if (currentUser.IsAuthenticated && currentUser.UserId.HasValue)
		{
			context.TeamMembers.Add(new TeamMember
			{
				Id = Guid.NewGuid(),
				TenantId = tenantContext.TenantId,
				TeamId = team.Id,
				UserId = currentUser.UserId.Value,
				Role = TeamMemberRole.Lead
			});
		}
		return new TeamDto(team.Id, team.Name, team.Key, team.Description, team.Icon, team.Color, team.IsArchived, team.CreatedAtUtc);
	}
}
