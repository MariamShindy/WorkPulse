using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class Project : TenantEntity
{
	public Guid TeamId { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Key { get; set; } = string.Empty;

	public string? Description { get; set; }

	public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

	public Guid? LeadId { get; set; }

	public DateOnly? StartDate { get; set; }

	public DateOnly? TargetDate { get; set; }

	public bool IsArchived { get; set; }
}
