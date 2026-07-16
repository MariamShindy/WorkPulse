using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Collaboration.Queries.ListTaskActivity;

public sealed class ListTaskActivityQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskActivityQuery, Result<PagedList<TaskActivityDto>>>
{
	public async Task<Result<PagedList<TaskActivityDto>>> Handle(ListTaskActivityQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<TaskActivity> query = from a in context.TaskActivities.AsNoTracking()
			where a.TaskId == request.TaskId
			orderby a.CreatedAtUtc descending
			select a;
		return new PagedList<TaskActivityDto>(totalCount: await query.CountAsync(ct), items: await (from a in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new TaskActivityDto(a.Id, a.TaskId, a.ActorId, a.Type.ToString(), a.Summary, a.MetadataJson, a.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
