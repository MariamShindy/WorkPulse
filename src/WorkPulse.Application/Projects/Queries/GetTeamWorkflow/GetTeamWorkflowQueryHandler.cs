using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.GetTeamWorkflow;

public sealed class GetTeamWorkflowQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetTeamWorkflowQuery, Result<WorkflowDto>>
{
	public async Task<Result<WorkflowDto>> Handle(GetTeamWorkflowQuery request, CancellationToken ct)
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
		return new WorkflowDto(States: await (from s in context.WorkflowStates.AsNoTracking()
			where s.WorkflowId == workflow.Id
			orderby s.Position
			select new WorkflowStateDto(s.Id, s.Name, s.Type.ToString(), s.Color, s.Position, s.IsDefault)).ToListAsync(ct), Id: workflow.Id, TeamId: workflow.TeamId, Name: workflow.Name, IsDefault: workflow.IsDefault);
	}
}
