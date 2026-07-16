using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		string text = request.Sort?.SortBy?.ToLowerInvariant();
		if (1 == 0)
		{
		}
		string text2 = text;
		IOrderedQueryable<AutomationRule> orderedQueryable = ((text2 == "name") ? (request.Sort.IsDescending ? query.OrderByDescending((AutomationRule r) => r.Name) : query.OrderBy((AutomationRule r) => r.Name)) : ((!(text2 == "createdat")) ? query.OrderBy((AutomationRule r) => r.Name) : (request.Sort.IsDescending ? query.OrderByDescending((AutomationRule r) => r.CreatedAtUtc) : query.OrderBy((AutomationRule r) => r.CreatedAtUtc))));
		if (1 == 0)
		{
		}
		query = orderedQueryable;
		return new PagedList<AutomationRuleDto>(totalCount: await query.CountAsync(ct), items: await (from r in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new AutomationRuleDto(r.Id, r.Name, r.TriggerType.ToString(), r.TriggerConfigJson, r.ActionType.ToString(), r.ActionConfigJson, r.IsEnabled, r.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
