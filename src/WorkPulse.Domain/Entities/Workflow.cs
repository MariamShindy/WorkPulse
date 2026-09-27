using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class Workflow : TenantEntity
{
	public Guid TeamId { get; set; }

	public string Name { get; set; } = "Default";

	public bool IsDefault { get; set; } = true;
}
