using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Automation.Queries;

public sealed class ListAutomationRulesQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListAutomationRulesQuery, Result<PagedList<AutomationRuleDto>>>
{
	public async Task<Result<PagedList<AutomationRuleDto>>> Handle(ListAutomationRulesQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IQueryable<AutomationRule> query = context.AutomationRules.AsNoTracking().ForTenant(tenantContext);
		string? sortBy = request.Sort?.SortBy?.ToLowerInvariant();
		bool isDescending = request.Sort?.IsDescending == true;
		query = sortBy switch
		{
			"name" => isDescending ? query.OrderByDescending((AutomationRule r) => r.Name) : query.OrderBy((AutomationRule r) => r.Name),
			"createdat" => isDescending ? query.OrderByDescending((AutomationRule r) => r.CreatedAtUtc) : query.OrderBy((AutomationRule r) => r.CreatedAtUtc),
			_ => query.OrderBy((AutomationRule r) => r.Name)
		};
		return new PagedList<AutomationRuleDto>(totalCount: await query.CountAsync(ct), items: await (from r in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new AutomationRuleDto(r.Id, r.Name, r.TriggerType.ToString(), r.TriggerConfigJson, r.ActionType.ToString(), r.ActionConfigJson, r.IsEnabled, r.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
