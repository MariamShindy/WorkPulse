using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.WorkManagement.Commands.CreateLabel;
using WorkPulse.Application.WorkManagement.Commands.DeleteLabel;
using WorkPulse.Application.WorkManagement.Commands.UpdateLabel;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.ListLabels;
using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/labels")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class LabelsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<LabelDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<LabelDto>>>)new ListLabelsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<LabelDto>> Create([FromBody] CreateLabelRequest request, CancellationToken ct)
	{
		Result<LabelDto> result = await sender.Send((IRequest<Result<LabelDto>>)new CreateLabelCommand(request.Name, request.Color), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("List", result.Value);
	}

	[HttpPut("{labelId:guid}")]
	public async Task<ActionResult<LabelDto>> Update(Guid labelId, [FromBody] UpdateLabelRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<LabelDto>>)new UpdateLabelCommand(labelId, request.Name, request.Color), ct)).ToActionResult();
	}

	[HttpDelete("{labelId:guid}")]
	public async Task<IActionResult> Delete(Guid labelId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteLabelCommand(labelId), ct)).ToActionResult();
	}
}
