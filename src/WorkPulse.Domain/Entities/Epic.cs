using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class Epic : TenantEntity
{
	public Guid TeamId { get; set; }

	public string Title { get; set; } = string.Empty;

	public string? Description { get; set; }

	public EpicStatus Status { get; set; } = EpicStatus.Open;
}
