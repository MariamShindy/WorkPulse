using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.DeleteWorkflowState;

public sealed class DeleteWorkflowStateCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteWorkflowStateCommand, Result>
{
	public async Task<Result> Handle(DeleteWorkflowStateCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Workflow workflow = await context.Workflows.AsNoTracking().FirstOrDefaultAsync((Workflow w) => w.TeamId == request.TeamId && w.IsDefault, ct);
		if (workflow == null)
		{
			return Error.NotFound("Workflow.NotFound", "Workflow not found.");
		}
		WorkflowState state = await context.WorkflowStates.FirstOrDefaultAsync((WorkflowState s) => s.Id == request.StateId && s.WorkflowId == workflow.Id, ct);
		if (state == null)
		{
			return Error.NotFound("Workflow.StateNotFound", "Workflow state not found.");
		}
		if (state.IsDefault)
		{
			return Error.Validation("Workflow.DefaultStateRequired", "Cannot delete the default workflow state.");
		}
		if (await context.TaskItems.AnyAsync((TaskItem t) => t.WorkflowStateId == state.Id, ct))
		{
			return Error.Conflict("Workflow.StateInUse", "Workflow state is in use by tasks.");
		}
		context.WorkflowStates.Remove(state);
		return Result.Success();
	}
}
