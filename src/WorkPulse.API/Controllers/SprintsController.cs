using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.WorkManagement.Commands.CreateSprint;
using WorkPulse.Application.WorkManagement.Commands.DeleteSprint;
using WorkPulse.Application.WorkManagement.Commands.UpdateSprint;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.GetSprint;
using WorkPulse.Application.WorkManagement.Queries.GetSprintBacklog;
using WorkPulse.Application.WorkManagement.Queries.ListSprints;
using WorkPulse.Domain.Enums;
using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/sprints")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class SprintsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<SprintDto>>> List([FromQuery] Guid? teamId, [FromQuery] SprintStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<SprintDto>>>)new ListSprintsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, teamId, status), ct)).ToActionResult();
	}

	[HttpGet("{sprintId:guid}")]
	public async Task<ActionResult<SprintDto>> Get(Guid sprintId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<SprintDto>>)new GetSprintQuery(sprintId), ct)).ToActionResult();
	}

	[HttpGet("{sprintId:guid}/backlog")]
	public async Task<ActionResult<PagedList<TaskItemDto>>> Backlog(Guid sprintId, [FromQuery] Guid teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TaskItemDto>>>)new GetSprintBacklogQuery(teamId, sprintId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpGet("backlog")]
	public async Task<ActionResult<PagedList<TaskItemDto>>> TeamBacklog([FromQuery] Guid teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TaskItemDto>>>)new GetSprintBacklogQuery(teamId, null, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<SprintDto>> Create([FromBody] CreateSprintRequest request, CancellationToken ct)
	{
		Result<SprintDto> result = await sender.Send((IRequest<Result<SprintDto>>)new CreateSprintCommand(request.TeamId, request.Name, request.Goal, request.StartDate, request.EndDate, request.Status), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			sprintId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{sprintId:guid}")]
	public async Task<ActionResult<SprintDto>> Update(Guid sprintId, [FromBody] UpdateSprintRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<SprintDto>>)new UpdateSprintCommand(sprintId, request.Name, request.Goal, request.StartDate, request.EndDate, request.Status), ct)).ToActionResult();
	}

	[HttpDelete("{sprintId:guid}")]
	public async Task<IActionResult> Delete(Guid sprintId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteSprintCommand(sprintId), ct)).ToActionResult();
	}
}
