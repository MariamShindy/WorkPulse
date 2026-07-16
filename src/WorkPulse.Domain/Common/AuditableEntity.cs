using System;

namespace WorkPulse.Domain.Common;

public abstract class AuditableEntity : IAuditableEntity
{
	public DateTime CreatedAtUtc { get; set; }

	public Guid? CreatedById { get; set; }

	public DateTime? UpdatedAtUtc { get; set; }

	public Guid? UpdatedById { get; set; }
}
