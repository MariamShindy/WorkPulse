using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		TeamMember member = await context.TeamMembers.FirstOrDefaultAsync((TeamMember m) => m.Id == request.MemberId && m.TeamId == request.TeamId, ct);
		if (member == null)
		{
			return Error.NotFound("Team.MemberNotFound", "Team member not found.");
		}
		member.Role = request.Role;
		return Result.Success();
	}
}
