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

namespace WorkPulse.Application.WorkManagement.Queries.ListSprints;

public sealed class ListSprintsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListSprintsQuery, Result<PagedList<SprintDto>>>
{
	public async Task<Result<PagedList<SprintDto>>> Handle(ListSprintsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IQueryable<Sprint> query = context.Sprints.AsNoTracking().ForTenant(tenantContext);
		if (request.TeamId.HasValue)
		{
			query = query.Where((Sprint s) => s.TeamId == request.TeamId.Value);
		}
		if (request.Status.HasValue)
		{
			query = query.Where((Sprint s) => (int)s.Status == (int)request.Status.Value);
		}
		query = query.OrderByDescending((Sprint s) => s.StartDate);
		return new PagedList<SprintDto>(totalCount: await query.CountAsync(ct), items: await (from s in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new SprintDto(s.Id, s.TeamId, s.Name, s.Goal, s.StartDate, s.EndDate, s.Status.ToString(), s.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
