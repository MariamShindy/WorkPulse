
namespace WorkPulse.Application.Organizations.Commands.RemoveTeamMember;

public sealed class RemoveTeamMemberCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<RemoveTeamMemberCommand, Result>
{
	public async Task<Result> Handle(RemoveTeamMemberCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TeamMember? member = await context.TeamMembers.FirstOrDefaultAsync((TeamMember m) => m.Id == request.MemberId && m.TeamId == request.TeamId, ct);
		if (member is null)
		{
			return Error.NotFound("Team.MemberNotFound", "Team member not found.");
		}
		context.TeamMembers.Remove(member);
		return Result.Success();
	}
}
