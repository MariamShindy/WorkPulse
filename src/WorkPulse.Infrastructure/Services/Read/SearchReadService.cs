using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Search.Dtos;

namespace WorkPulse.Infrastructure.Services.Read;

public sealed class SearchReadService(ApplicationDbContext context) : ISearchReadService
{
	public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(Guid tenantId, string query, SearchScope scope, int limit, CancellationToken ct)
	{
		string pattern = "%" + query + "%";
		List<SearchResultDto> results = new List<SearchResultDto>();
		SearchScope searchScope = scope;
		if ((uint)searchScope <= 1u)
		{
			results.AddRange(await (from t in context.TaskItems.AsNoTracking()
				join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
				where t.TenantId == tenantId && team.TenantId == tenantId && (EF.Functions.ILike(t.Title, pattern) || (t.Description != null && EF.Functions.ILike(t.Description, pattern)) || EF.Functions.ILike(string.Concat(team.Key + "-", t.Number.ToString()), pattern))
				orderby t.UpdatedAtUtc ?? t.CreatedAtUtc descending
				select new SearchResultDto(t.Id, "Task", string.Concat(string.Concat(string.Concat(team.Key + "-", t.Number), " "), t.Title), t.Description, t.Title, 1.0, t.TeamId, t.ProjectId)).Take(limit).ToListAsync(ct));
		}
		searchScope = scope;
		if ((searchScope == SearchScope.All || searchScope == SearchScope.Projects) ? true : false)
		{
			int remaining = limit - results.Count;
			if (remaining > 0)
			{
				results.AddRange(await (from p in (from p in context.Projects.AsNoTracking()
						where p.TenantId == tenantId && (EF.Functions.ILike(p.Name, pattern) || EF.Functions.ILike(p.Key, pattern) || (p.Description != null && EF.Functions.ILike(p.Description, pattern)))
						orderby p.UpdatedAtUtc ?? p.CreatedAtUtc descending
						select p).Take(remaining)
					select new SearchResultDto(p.Id, "Project", string.Concat(p.Key + " — ", p.Name), p.Description, p.Name, 0.9, p.TeamId, p.Id)).ToListAsync(ct));
			}
		}
		searchScope = scope;
		if ((searchScope == SearchScope.All || searchScope == SearchScope.Teams) ? true : false)
		{
			int remaining2 = limit - results.Count;
			if (remaining2 > 0)
			{
				results.AddRange(await (from t in (from t in context.Teams.AsNoTracking()
						where t.TenantId == tenantId && !t.IsArchived && (EF.Functions.ILike(t.Name, pattern) || EF.Functions.ILike(t.Key, pattern))
						orderby t.Name
						select t).Take(remaining2)
					select new SearchResultDto(t.Id, "Team", string.Concat(t.Key + " — ", t.Name), t.Description, t.Name, 0.8, t.Id, null)).ToListAsync(ct));
			}
		}
		searchScope = scope;
		if ((searchScope == SearchScope.All || searchScope == SearchScope.Comments) ? true : false)
		{
			int remaining3 = limit - results.Count;
			if (remaining3 > 0)
			{
				results.AddRange(await (from c in context.TaskComments.AsNoTracking()
					join t in context.TaskItems.AsNoTracking() on c.TaskId equals t.Id
					join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
					where c.TenantId == tenantId && t.TenantId == tenantId && team.TenantId == tenantId && EF.Functions.ILike(c.Body, pattern)
					orderby c.CreatedAtUtc descending
					select new SearchResultDto(c.Id, "Comment", string.Concat(string.Concat("Comment on " + team.Key, "-"), t.Number), c.Body, c.Body, 0.7, t.TeamId, t.ProjectId)).Take(remaining3).ToListAsync(ct));
			}
		}
		return results.OrderByDescending((SearchResultDto r) => r.Rank).Take(limit).ToList();
	}
}
