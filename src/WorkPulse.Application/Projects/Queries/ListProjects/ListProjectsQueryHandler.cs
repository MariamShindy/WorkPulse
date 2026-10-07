using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;

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
		if (request.ArchivedOnly)
		{
			query = query.Where(x => x.p.IsArchived);
		}
		else if (!request.IncludeArchived)
		{
			query = query.Where(x => !x.p.IsArchived);
		}
		string? sortBy = request.Sort?.SortBy?.ToLowerInvariant();
		bool isDescending = request.Sort?.IsDescending == true;
		// When archived rows are included, surface them first so the filter change is obvious.
		bool prioritizeArchived = request.IncludeArchived && !request.ArchivedOnly;
		query = sortBy switch
		{
			"createdat" => isDescending
				? (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenByDescending(x => x.p.CreatedAtUtc) : query.OrderByDescending(x => x.p.CreatedAtUtc))
				: (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenBy(x => x.p.CreatedAtUtc) : query.OrderBy(x => x.p.CreatedAtUtc)),
			"status" => isDescending
				? (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenByDescending(x => x.p.Status) : query.OrderByDescending(x => x.p.Status))
				: (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenBy(x => x.p.Status) : query.OrderBy(x => x.p.Status)),
			_ => isDescending
				? (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenByDescending(x => x.p.Name) : query.OrderByDescending(x => x.p.Name))
				: (prioritizeArchived ? query.OrderByDescending(x => x.p.IsArchived).ThenBy(x => x.p.Name) : query.OrderBy(x => x.p.Name))
		};
		return new PagedList<ProjectDto>(totalCount: await query.CountAsync(ct), items: await (from x in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new ProjectDto(x.p.Id, x.p.TeamId, x.t.Key, x.p.Name, x.p.Key, x.p.Description, x.p.Status.ToString(), x.p.LeadId, x.p.StartDate, x.p.TargetDate, x.p.IsArchived, x.p.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
