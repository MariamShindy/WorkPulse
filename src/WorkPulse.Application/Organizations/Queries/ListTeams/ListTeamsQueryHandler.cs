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

namespace WorkPulse.Application.Organizations.Queries.ListTeams;

public sealed class ListTeamsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTeamsQuery, Result<PagedList<TeamDto>>>
{
	public async Task<Result<PagedList<TeamDto>>> Handle(ListTeamsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IQueryable<Team> query = context.Teams.AsNoTracking().ForTenant(tenantContext);
		if (!request.IncludeArchived)
		{
			query = query.Where((Team t) => !t.IsArchived);
		}
		string text = request.Sort?.SortBy?.ToLowerInvariant();
		if (1 == 0)
		{
		}
		string text2 = text;
		IOrderedQueryable<Team> orderedQueryable = ((text2 == "key") ? (request.Sort.IsDescending ? query.OrderByDescending((Team t) => t.Key) : query.OrderBy((Team t) => t.Key)) : ((!(text2 == "createdat")) ? ((request.Sort?.IsDescending ?? false) ? query.OrderByDescending((Team t) => t.Name) : query.OrderBy((Team t) => t.Name)) : (request.Sort.IsDescending ? query.OrderByDescending((Team t) => t.CreatedAtUtc) : query.OrderBy((Team t) => t.CreatedAtUtc))));
		if (1 == 0)
		{
		}
		query = orderedQueryable;
		return new PagedList<TeamDto>(totalCount: await query.CountAsync(ct), items: await (from t in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new TeamDto(t.Id, t.Name, t.Key, t.Description, t.Icon, t.Color, t.IsArchived, t.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
