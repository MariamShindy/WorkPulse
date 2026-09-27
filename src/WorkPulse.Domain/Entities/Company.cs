using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class Company : AuditableEntity
{
	public Guid Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Slug { get; set; } = string.Empty;

	public string? LogoUrl { get; set; }

	public string? Description { get; set; }

	public bool IsActive { get; set; } = true;
}
