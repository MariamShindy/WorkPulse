using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Commands.CreateWorkflowState;
using WorkPulse.Application.Projects.Commands.DeleteWorkflowState;
using WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;
using WorkPulse.Application.Projects.Commands.UpdateWorkflowState;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Queries.GetTeamWorkflow;

using WorkPulse.API.Contracts.Projects;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/teams/{teamId:guid}/workflow")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class WorkflowsController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<WorkflowDto>> Get(Guid teamId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<WorkflowDto>>)new GetTeamWorkflowQuery(teamId), ct)).ToActionResult();
	}

	[HttpPost("states")]
	public async Task<ActionResult<WorkflowStateDto>> CreateState(Guid teamId, [FromBody] CreateWorkflowStateRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<WorkflowStateDto>>)new CreateWorkflowStateCommand(teamId, request.Name, request.Type, request.Color, request.Position), ct)).ToActionResult();
	}

	[HttpPut("states/{stateId:guid}")]
	public async Task<ActionResult<WorkflowStateDto>> UpdateState(Guid teamId, Guid stateId, [FromBody] UpdateWorkflowStateRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<WorkflowStateDto>>)new UpdateWorkflowStateCommand(teamId, stateId, request.Name, request.Type, request.Color, request.Position, request.IsDefault), ct)).ToActionResult();
	}

	[HttpDelete("states/{stateId:guid}")]
	public async Task<IActionResult> DeleteState(Guid teamId, Guid stateId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteWorkflowStateCommand(teamId, stateId), ct)).ToActionResult();
	}

	[HttpPut("states/reorder")]
	public async Task<IActionResult> Reorder(Guid teamId, [FromBody] ReorderWorkflowStatesRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new ReorderWorkflowStatesCommand(teamId, request.StateIdsInOrder), ct)).ToActionResult();
	}
}
