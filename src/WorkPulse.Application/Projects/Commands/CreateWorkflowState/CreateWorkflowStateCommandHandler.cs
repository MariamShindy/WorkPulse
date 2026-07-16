using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.CreateWorkflowState;

public sealed class CreateWorkflowStateCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateWorkflowStateCommand, Result<WorkflowStateDto>>
{
	public async Task<Result<WorkflowStateDto>> Handle(CreateWorkflowStateCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Workflow workflow = await context.Workflows.FirstOrDefaultAsync((Workflow w) => w.TeamId == request.TeamId && w.IsDefault, ct);
		if (workflow == null)
		{
			return Error.NotFound("Workflow.NotFound", "Workflow not found.");
		}
		WorkflowState state = new WorkflowState
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			WorkflowId = workflow.Id,
			Name = request.Name.Trim(),
			Type = request.Type,
			Color = request.Color,
			Position = request.Position
		};
		context.WorkflowStates.Add(state);
		return new WorkflowStateDto(state.Id, state.Name, state.Type.ToString(), state.Color, state.Position, state.IsDefault);
	}
}
