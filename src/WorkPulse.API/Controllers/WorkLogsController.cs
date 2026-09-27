using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.WorkManagement.Commands.AddWorkLog;
using WorkPulse.Application.WorkManagement.Commands.DeleteWorkLog;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.ListWorkLogs;
using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/work-logs")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class WorkLogsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<WorkLogDto>>> List([FromQuery] Guid taskId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<WorkLogDto>>>)new ListWorkLogsQuery(taskId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<WorkLogDto>> Create([FromBody] CreateWorkLogRequest request, CancellationToken ct)
	{
		Result<WorkLogDto> result = await sender.Send((IRequest<Result<WorkLogDto>>)new AddWorkLogCommand(request.TaskId, request.Hours, request.Description, request.LoggedDate), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("List", new
		{
			taskId = request.TaskId
		}, result.Value);
	}

	[HttpDelete("{workLogId:guid}")]
	public async Task<IActionResult> Delete(Guid workLogId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteWorkLogCommand(workLogId), ct)).ToActionResult();
	}
}
