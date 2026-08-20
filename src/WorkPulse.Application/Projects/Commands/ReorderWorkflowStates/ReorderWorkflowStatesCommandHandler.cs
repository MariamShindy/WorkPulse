
namespace WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;

public sealed class ReorderWorkflowStatesCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ReorderWorkflowStatesCommand, Result>
{
	public async Task<Result> Handle(ReorderWorkflowStatesCommand request, CancellationToken ct)
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
		List<WorkflowState> states = await context.WorkflowStates.Where((WorkflowState s) => s.WorkflowId == workflow.Id).ToListAsync(ct);
		int i;
		for (i = 0; i < request.StateIdsInOrder.Count; i++)
		{
			WorkflowState? state = states.FirstOrDefault((WorkflowState s) => s.Id == request.StateIdsInOrder[i]);
			if (state is not null)
			{
				state.Position = i;
			}
		}
		return Result.Success();
	}
}
