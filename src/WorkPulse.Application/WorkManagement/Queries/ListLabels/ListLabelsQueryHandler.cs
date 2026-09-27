using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.ListLabels;

public sealed class ListLabelsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListLabelsQuery, Result<PagedList<LabelDto>>>
{
	public async Task<Result<PagedList<LabelDto>>> Handle(ListLabelsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<Label> query = context.Labels.AsNoTracking().ForTenant(tenantContext).OrderBy((Label l) => l.Name);
		return new PagedList<LabelDto>(totalCount: await query.CountAsync(ct), items: await (from l in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new LabelDto(l.Id, l.Name, l.Color, l.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
