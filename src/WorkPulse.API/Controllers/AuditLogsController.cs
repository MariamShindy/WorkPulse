using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Audit.Dtos;
using WorkPulse.Application.Audit.Queries;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class AuditLogsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<AuditLogEntryDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? sortBy = null, [FromQuery] string sortDirection = "desc", [FromQuery] string? entityType = null, [FromQuery] Guid? entityId = null, [FromQuery] Guid? userId = null, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<AuditLogEntryDto>>>)new ListAuditLogsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, new SortParams
		{
			SortBy = sortBy,
			SortDirection = sortDirection
		}, entityType, entityId, userId), ct)).ToActionResult();
	}
}
