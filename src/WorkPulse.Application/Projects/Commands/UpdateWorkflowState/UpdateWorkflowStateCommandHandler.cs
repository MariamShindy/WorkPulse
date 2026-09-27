using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.UpdateWorkflowState;

public sealed class UpdateWorkflowStateCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateWorkflowStateCommand, Result<WorkflowStateDto>>
{
	public async Task<Result<WorkflowStateDto>> Handle(UpdateWorkflowStateCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Workflow? workflow = await context.Workflows.AsNoTracking().FirstOrDefaultAsync((Workflow w) => w.TeamId == request.TeamId && w.IsDefault, ct);
		if (workflow is null)
		{
			return Error.NotFound("Workflow.NotFound", "Workflow not found.");
		}
		WorkflowState? state = await context.WorkflowStates.FirstOrDefaultAsync((WorkflowState s) => s.Id == request.StateId && s.WorkflowId == workflow.Id, ct);
		if (state is null)
		{
			return Error.NotFound("Workflow.StateNotFound", "Workflow state not found.");
		}
		if (request.IsDefault)
		{
			foreach (WorkflowState current in await context.WorkflowStates.Where((WorkflowState s) => s.WorkflowId == workflow.Id && s.IsDefault && s.Id != state.Id).ToListAsync(ct))
			{
				current.IsDefault = false;
			}
		}
		state.Name = request.Name.Trim();
		state.Type = request.Type;
		state.Color = request.Color;
		state.Position = request.Position;
		state.IsDefault = request.IsDefault;
		return new WorkflowStateDto(state.Id, state.Name, state.Type.ToString(), state.Color, state.Position, state.IsDefault);
	}
}
