
namespace WorkPulse.Application.Organizations.Commands.UpdateTeamMember;

public sealed class UpdateTeamMemberCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateTeamMemberCommand, Result>
{
	public async Task<Result> Handle(UpdateTeamMemberCommand request, CancellationToken ct)
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
		member.Role = request.Role;
		return Result.Success();
	}
}
