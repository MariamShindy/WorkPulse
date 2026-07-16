using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class Label : TenantEntity
{
	public string Name { get; set; } = string.Empty;

	public string Color { get; set; } = string.Empty;
}
