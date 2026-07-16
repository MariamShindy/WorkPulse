using System;
using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class WorkLog : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid UserId { get; set; }

	public decimal Hours { get; set; }

	public string? Description { get; set; }

	public DateOnly LoggedDate { get; set; }
}
