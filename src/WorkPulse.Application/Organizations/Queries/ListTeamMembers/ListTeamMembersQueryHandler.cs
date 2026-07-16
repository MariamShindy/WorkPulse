using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Organizations.Queries.ListTeamMembers;

public sealed class ListTeamMembersQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTeamMembersQuery, Result<PagedList<TeamMemberDto>>>
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
		IOrderedQueryable<TeamMember> query = from m in context.TeamMembers.AsNoTracking()
			where m.TeamId == request.TeamId
			orderby m.CreatedAtUtc
			select m;
		return new PagedList<TeamMemberDto>(totalCount: await query.CountAsync(ct), items: await (from m in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new TeamMemberDto(m.Id, m.TeamId, m.UserId, string.Empty, string.Empty, m.Role.ToString(), m.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
