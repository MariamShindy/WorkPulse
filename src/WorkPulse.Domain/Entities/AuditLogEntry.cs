using System;
using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class AuditLogEntry : TenantEntity
{
	public string EntityType { get; set; } = string.Empty;

	public Guid EntityId { get; set; }

	public string Action { get; set; } = string.Empty;

	public Guid? UserId { get; set; }

	public string? ChangesJson { get; set; }

	public DateTime Timestamp { get; set; }
}
