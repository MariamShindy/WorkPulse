using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class Team : TenantEntity
{
	public string Name { get; set; } = string.Empty;

	public string Key { get; set; } = string.Empty;

	public string? Description { get; set; }

	public string? Icon { get; set; }

	public string? Color { get; set; }

	public bool IsArchived { get; set; }
}
