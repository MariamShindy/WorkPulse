namespace WorkPulse.Application.Organizations.Services;

public static class DefaultWorkflowFactory
{
	public static (Workflow Workflow, IReadOnlyList<WorkflowState> States) Create(Guid tenantId, Guid teamId)
	{
		Guid workflowId = Guid.NewGuid();
		Workflow workflow = new Workflow
		{
			Id = workflowId,
			TenantId = tenantId,
			TeamId = teamId,
			Name = "Default",
			IsDefault = true
		};

		List<WorkflowState> states =
		[
			CreateState(tenantId, workflowId, "Backlog", WorkflowStateType.Backlog, "#9CA3AF", 0),
			CreateState(tenantId, workflowId, "Todo", WorkflowStateType.Unstarted, "#6B7280", 1, isDefault: true),
			CreateState(tenantId, workflowId, "In Progress", WorkflowStateType.Started, "#3B82F6", 2),
			CreateState(tenantId, workflowId, "In Review", WorkflowStateType.Started, "#8B5CF6", 3),
			CreateState(tenantId, workflowId, "Done", WorkflowStateType.Completed, "#10B981", 4),
			CreateState(tenantId, workflowId, "Canceled", WorkflowStateType.Cancelled, "#EF4444", 5)
		];

		return (Workflow: workflow, States: states);
	}

	private static WorkflowState CreateState(
		Guid tenantId,
		Guid workflowId,
		string name,
		WorkflowStateType type,
		string color,
		int position,
		bool isDefault = false)
	{
		return new WorkflowState
		{
			Id = Guid.NewGuid(),
			TenantId = tenantId,
			WorkflowId = workflowId,
			Name = name,
			Type = type,
			Color = color,
			Position = position,
			IsDefault = isDefault
		};
	}
}
