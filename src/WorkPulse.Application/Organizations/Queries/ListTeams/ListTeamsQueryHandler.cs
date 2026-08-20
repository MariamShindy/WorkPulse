using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Organizations.Dtos;

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
		string? sortBy = request.Sort?.SortBy?.ToLowerInvariant();
		bool isDescending = request.Sort?.IsDescending == true;
		query = sortBy switch
		{
			"key" => isDescending ? query.OrderByDescending((Team t) => t.Key) : query.OrderBy((Team t) => t.Key),
			"createdat" => isDescending ? query.OrderByDescending((Team t) => t.CreatedAtUtc) : query.OrderBy((Team t) => t.CreatedAtUtc),
			_ => isDescending ? query.OrderByDescending((Team t) => t.Name) : query.OrderBy((Team t) => t.Name)
		};
		return new PagedList<TeamDto>(totalCount: await query.CountAsync(ct), items: await (from t in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new TeamDto(t.Id, t.Name, t.Key, t.Description, t.Icon, t.Color, t.IsArchived, t.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
