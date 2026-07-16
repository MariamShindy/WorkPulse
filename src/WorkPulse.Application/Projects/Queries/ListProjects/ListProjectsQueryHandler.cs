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
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Queries.ListProjects;

public sealed class ListProjectsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListProjectsQuery, Result<PagedList<ProjectDto>>>
{
	public async Task<Result<PagedList<ProjectDto>>> Handle(ListProjectsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		var query = from p in context.Projects.AsNoTracking().ForTenant(tenantContext)
			join t in context.Teams.AsNoTracking().ForTenant(tenantContext) on p.TeamId equals t.Id
			select new { p, t };
		if (request.TeamId.HasValue)
		{
			query = query.Where(x => x.p.TeamId == request.TeamId.Value);
		}
		if (request.Status.HasValue)
		{
			query = query.Where(x => (int)x.p.Status == (int)request.Status.Value);
		}
		if (!request.IncludeArchived)
		{
			query = query.Where(x => !x.p.IsArchived);
		}
		string text = request.Sort?.SortBy?.ToLowerInvariant();
		if (1 == 0)
		{
		}
		string text2 = text;
		var orderedQueryable = ((text2 == "createdat") ? (request.Sort.IsDescending ? query.OrderByDescending(x => x.p.CreatedAtUtc) : query.OrderBy(x => x.p.CreatedAtUtc)) : ((!(text2 == "status")) ? ((request.Sort?.IsDescending ?? false) ? query.OrderByDescending(x => x.p.Name) : query.OrderBy(x => x.p.Name)) : (request.Sort.IsDescending ? query.OrderByDescending(x => x.p.Status) : query.OrderBy(x => x.p.Status))));
		if (1 == 0)
		{
		}
		query = orderedQueryable;
		return new PagedList<ProjectDto>(totalCount: await query.CountAsync(ct), items: await (from x in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new ProjectDto(x.p.Id, x.p.TeamId, x.t.Key, x.p.Name, x.p.Key, x.p.Description, x.p.Status.ToString(), x.p.LeadId, x.p.StartDate, x.p.TargetDate, x.p.IsArchived, x.p.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
