using WorkPulse.Application.Audit.Dtos;
using WorkPulse.Application.Common.Pagination;

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
		string? sortBy = request.Sort?.SortBy?.ToLowerInvariant();
		bool isDescending = request.Sort?.IsDescending == true;
		query = sortBy switch
		{
			"entitytype" => isDescending ? query.OrderByDescending((AuditLogEntry a) => a.EntityType) : query.OrderBy((AuditLogEntry a) => a.EntityType),
			"action" => isDescending ? query.OrderByDescending((AuditLogEntry a) => a.Action) : query.OrderBy((AuditLogEntry a) => a.Action),
			_ => query.OrderByDescending((AuditLogEntry a) => a.Timestamp)
		};
		return new PagedList<AuditLogEntryDto>(totalCount: await query.CountAsync(ct), items: await (from a in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new AuditLogEntryDto(a.Id, a.EntityType, a.EntityId, a.Action, a.UserId, a.ChangesJson, a.Timestamp)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
