using WorkPulse.Application.Audit.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Audit.Queries;

public sealed record ListAuditLogsQuery(PaginationParams Pagination, SortParams? Sort = null, string? EntityType = null, Guid? EntityId = null, Guid? UserId = null) : IRequest<Result<PagedList<AuditLogEntryDto>>>, IBaseRequest, IQuery;
