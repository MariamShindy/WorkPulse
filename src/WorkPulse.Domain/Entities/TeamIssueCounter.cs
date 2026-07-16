using System;

namespace WorkPulse.Domain.Entities;

public sealed class TeamIssueCounter
{
	public Guid TeamId { get; set; }

	public Guid TenantId { get; set; }

	public int LastNumber { get; set; }
}
