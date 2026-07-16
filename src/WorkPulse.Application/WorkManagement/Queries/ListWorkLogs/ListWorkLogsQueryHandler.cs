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

namespace WorkPulse.Application.WorkManagement.Queries.ListWorkLogs;

public sealed class ListWorkLogsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListWorkLogsQuery, Result<PagedList<WorkLogDto>>>
{
	public async Task<Result<PagedList<WorkLogDto>>> Handle(ListWorkLogsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<WorkLog> query = from w in context.WorkLogs.AsNoTracking()
			where w.TaskId == request.TaskId
			orderby w.LoggedDate descending, w.CreatedAtUtc descending
			select w;
		return new PagedList<WorkLogDto>(totalCount: await query.CountAsync(ct), items: await (from w in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new WorkLogDto(w.Id, w.TaskId, w.UserId, w.Hours, w.Description, w.LoggedDate, w.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
