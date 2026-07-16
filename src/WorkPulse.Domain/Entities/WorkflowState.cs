using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class WorkflowState : TenantEntity
{
	public Guid WorkflowId { get; set; }

	public string Name { get; set; } = string.Empty;

	public WorkflowStateType Type { get; set; }

	public string Color { get; set; } = "#6B7280";

	public int Position { get; set; }

	public bool IsDefault { get; set; }
}
