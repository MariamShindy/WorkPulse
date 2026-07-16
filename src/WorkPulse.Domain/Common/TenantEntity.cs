using System;

namespace WorkPulse.Domain.Common;

public abstract class TenantEntity : AuditableEntity, ITenantEntity, ISoftDeletable
{
	public Guid Id { get; set; }

	public Guid TenantId { get; set; }

	public bool IsDeleted { get; set; }

	public DateTime? DeletedAtUtc { get; set; }

	public Guid? DeletedById { get; set; }
}
