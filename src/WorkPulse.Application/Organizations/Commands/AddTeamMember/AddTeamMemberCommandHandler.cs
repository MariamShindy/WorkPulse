using System;
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

namespace WorkPulse.Application.Organizations.Commands.AddTeamMember;

public sealed class AddTeamMemberCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<AddTeamMemberCommand, Result<TeamMemberDto>>
{
	public async Task<Result<TeamMemberDto>> Handle(AddTeamMemberCommand request, CancellationToken ct)
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
		if (await context.TeamMembers.AnyAsync((TeamMember m) => m.TeamId == request.TeamId && m.UserId == request.UserId, ct))
		{
			return Error.Conflict("Team.MemberExists", "User is already a member of this team.");
		}
		TeamMember member = new TeamMember
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			UserId = request.UserId,
			Role = request.Role
		};
		context.TeamMembers.Add(member);
		return new TeamMemberDto(member.Id, member.TeamId, member.UserId, string.Empty, string.Empty, member.Role.ToString(), member.CreatedAtUtc);
	}
}
