using System;
using System.Collections.Generic;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Services;

public static class DefaultWorkflowFactory
{
	public static (Workflow Workflow, IReadOnlyList<WorkflowState> States) Create(Guid tenantId, Guid teamId)
	{
		Guid guid = Guid.NewGuid();
		Workflow item = new Workflow
		{
			Id = guid,
			TenantId = tenantId,
			TeamId = teamId,
			Name = "Default",
			IsDefault = true
		};
		List<WorkflowState> item2 = new List<WorkflowState>
		{
			CreateState(tenantId, guid, "Backlog", WorkflowStateType.Backlog, "#9CA3AF", 0),
			CreateState(tenantId, guid, "Todo", WorkflowStateType.Unstarted, "#6B7280", 1, isDefault: true),
			CreateState(tenantId, guid, "In Progress", WorkflowStateType.Started, "#3B82F6", 2),
			CreateState(tenantId, guid, "In Review", WorkflowStateType.Started, "#8B5CF6", 3),
			CreateState(tenantId, guid, "Done", WorkflowStateType.Completed, "#10B981", 4),
			CreateState(tenantId, guid, "Canceled", WorkflowStateType.Cancelled, "#EF4444", 5)
		};
		return (Workflow: item, States: item2);
	}

	private static WorkflowState CreateState(Guid tenantId, Guid workflowId, string name, WorkflowStateType type, string color, int position, bool isDefault = false)
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
