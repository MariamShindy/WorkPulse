using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Commands.CreateEpic;
using WorkPulse.Application.WorkManagement.Commands.DeleteEpic;
using WorkPulse.Application.WorkManagement.Commands.UpdateEpic;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.GetEpic;
using WorkPulse.Application.WorkManagement.Queries.ListEpics;
using WorkPulse.Domain.Enums;

using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/epics")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class EpicsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<EpicDto>>> List([FromQuery] Guid? teamId, [FromQuery] EpicStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<EpicDto>>>)new ListEpicsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, teamId, status), ct)).ToActionResult();
	}

	[HttpGet("{epicId:guid}")]
	public async Task<ActionResult<EpicDto>> Get(Guid epicId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<EpicDto>>)new GetEpicQuery(epicId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<EpicDto>> Create([FromBody] CreateEpicRequest request, CancellationToken ct)
	{
		Result<EpicDto> result = await sender.Send((IRequest<Result<EpicDto>>)new CreateEpicCommand(request.TeamId, request.Title, request.Description, request.Status), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			epicId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{epicId:guid}")]
	public async Task<ActionResult<EpicDto>> Update(Guid epicId, [FromBody] UpdateEpicRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<EpicDto>>)new UpdateEpicCommand(epicId, request.Title, request.Description, request.Status), ct)).ToActionResult();
	}

	[HttpDelete("{epicId:guid}")]
	public async Task<IActionResult> Delete(Guid epicId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteEpicCommand(epicId), ct)).ToActionResult();
	}
}
