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
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Queries.ListEpics;

public sealed class ListEpicsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListEpicsQuery, Result<PagedList<EpicDto>>>
{
	public async Task<Result<PagedList<EpicDto>>> Handle(ListEpicsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IQueryable<Epic> query = context.Epics.AsNoTracking().ForTenant(tenantContext);
		if (request.TeamId.HasValue)
		{
			query = query.Where((Epic e) => e.TeamId == request.TeamId.Value);
		}
		if (request.Status.HasValue)
		{
			query = query.Where((Epic e) => (int)e.Status == (int)request.Status.Value);
		}
		query = query.OrderByDescending((Epic e) => e.CreatedAtUtc);
		return new PagedList<EpicDto>(totalCount: await query.CountAsync(ct), items: await (from e in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new EpicDto(e.Id, e.TeamId, e.Title, e.Description, e.Status.ToString(), e.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
