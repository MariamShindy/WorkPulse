using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class Sprint : TenantEntity
{
	public Guid TeamId { get; set; }

	public string Name { get; set; } = string.Empty;

	public string? Goal { get; set; }

	public DateOnly StartDate { get; set; }

	public DateOnly EndDate { get; set; }

	public SprintStatus Status { get; set; } = SprintStatus.Planned;
}
