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
using WorkPulse.Application.WorkManagement.Commands.CreateSavedView;
using WorkPulse.Application.WorkManagement.Commands.DeleteSavedView;
using WorkPulse.Application.WorkManagement.Commands.UpdateSavedView;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.GetSavedView;
using WorkPulse.Application.WorkManagement.Queries.ListSavedViews;
using WorkPulse.Domain.Enums;

using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/saved-views")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class SavedViewsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<SavedViewDto>>> List([FromQuery] SavedViewEntityType? entityType, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<SavedViewDto>>>)new ListSavedViewsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, entityType), ct)).ToActionResult();
	}

	[HttpGet("{savedViewId:guid}")]
	public async Task<ActionResult<SavedViewDto>> Get(Guid savedViewId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<SavedViewDto>>)new GetSavedViewQuery(savedViewId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<SavedViewDto>> Create([FromBody] CreateSavedViewRequest request, CancellationToken ct)
	{
		Result<SavedViewDto> result = await sender.Send((IRequest<Result<SavedViewDto>>)new CreateSavedViewCommand(request.Name, request.EntityType, request.FiltersJson, request.SortJson, request.IsShared), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			savedViewId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{savedViewId:guid}")]
	public async Task<ActionResult<SavedViewDto>> Update(Guid savedViewId, [FromBody] UpdateSavedViewRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<SavedViewDto>>)new UpdateSavedViewCommand(savedViewId, request.Name, request.EntityType, request.FiltersJson, request.SortJson, request.IsShared), ct)).ToActionResult();
	}

	[HttpDelete("{savedViewId:guid}")]
	public async Task<IActionResult> Delete(Guid savedViewId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteSavedViewCommand(savedViewId), ct)).ToActionResult();
	}
}
