using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class TaskActivity : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid ActorId { get; set; }

	public ActivityType Type { get; set; }

	public string? Summary { get; set; }

	public string? MetadataJson { get; set; }
}
