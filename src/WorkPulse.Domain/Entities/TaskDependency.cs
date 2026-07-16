using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class TaskDependency : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid DependsOnTaskId { get; set; }

	public TaskDependencyType Type { get; set; }
}
