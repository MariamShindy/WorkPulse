using System;
using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class TaskAssignee : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid UserId { get; set; }
}
