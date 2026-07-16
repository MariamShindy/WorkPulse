using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Automation.Commands;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Automation.Queries;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;

using WorkPulse.API.Contracts.Automation;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/automation-rules")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class AutomationRulesController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<AutomationRuleDto>>> List([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? sortBy = null, [FromQuery] string sortDirection = "asc", CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<AutomationRuleDto>>>)new ListAutomationRulesQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, new SortParams
		{
			SortBy = sortBy,
			SortDirection = sortDirection
		}), ct)).ToActionResult();
	}

	[HttpGet("{ruleId:guid}")]
	public async Task<ActionResult<AutomationRuleDto>> Get(Guid ruleId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AutomationRuleDto>>)new GetAutomationRuleQuery(ruleId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<AutomationRuleDto>> Create([FromBody] CreateAutomationRuleRequest request, CancellationToken ct)
	{
		Result<AutomationRuleDto> result = await sender.Send((IRequest<Result<AutomationRuleDto>>)new CreateAutomationRuleCommand(request.Name, request.TriggerType, request.TriggerConfigJson, request.ActionType, request.ActionConfigJson, request.IsEnabled), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			ruleId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{ruleId:guid}")]
	public async Task<ActionResult<AutomationRuleDto>> Update(Guid ruleId, [FromBody] UpdateAutomationRuleRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AutomationRuleDto>>)new UpdateAutomationRuleCommand(ruleId, request.Name, request.TriggerType, request.TriggerConfigJson, request.ActionType, request.ActionConfigJson, request.IsEnabled), ct)).ToActionResult();
	}

	[HttpDelete("{ruleId:guid}")]
	public async Task<IActionResult> Delete(Guid ruleId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteAutomationRuleCommand(ruleId), ct)).ToActionResult();
	}
}
