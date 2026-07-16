using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Audit.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Audit.Queries;

public sealed class ListAuditLogsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListAuditLogsQuery, Result<PagedList<AuditLogEntryDto>>>
{
	public async Task<Result<PagedList<AuditLogEntryDto>>> Handle(ListAuditLogsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IQueryable<AuditLogEntry> query = context.AuditLogEntries.AsNoTracking().ForTenant(tenantContext);
		if (!string.IsNullOrWhiteSpace(request.EntityType))
		{
			query = query.Where((AuditLogEntry a) => a.EntityType == request.EntityType);
		}
		if (request.EntityId.HasValue)
		{
			query = query.Where((AuditLogEntry a) => a.EntityId == request.EntityId.Value);
		}
		if (request.UserId.HasValue)
		{
			query = query.Where((AuditLogEntry a) => a.UserId == request.UserId.Value);
		}
		string text = request.Sort?.SortBy?.ToLowerInvariant();
		if (1 == 0)
		{
		}
		string text2 = text;
		IOrderedQueryable<AuditLogEntry> orderedQueryable = ((text2 == "entitytype") ? (request.Sort.IsDescending ? query.OrderByDescending((AuditLogEntry a) => a.EntityType) : query.OrderBy((AuditLogEntry a) => a.EntityType)) : ((!(text2 == "action")) ? ((request.Sort?.IsDescending ?? false) ? query.OrderByDescending((AuditLogEntry a) => a.Timestamp) : query.OrderByDescending((AuditLogEntry a) => a.Timestamp)) : (request.Sort.IsDescending ? query.OrderByDescending((AuditLogEntry a) => a.Action) : query.OrderBy((AuditLogEntry a) => a.Action))));
		if (1 == 0)
		{
		}
		query = orderedQueryable;
		return new PagedList<AuditLogEntryDto>(totalCount: await query.CountAsync(ct), items: await (from a in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new AuditLogEntryDto(a.Id, a.EntityType, a.EntityId, a.Action, a.UserId, a.ChangesJson, a.Timestamp)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
