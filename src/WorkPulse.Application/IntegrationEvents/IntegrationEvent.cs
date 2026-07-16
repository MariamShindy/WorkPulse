using System;

namespace WorkPulse.Application.IntegrationEvents;

public abstract record IntegrationEvent
{
	public Guid EventId { get; init; } = Guid.NewGuid();

	public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
}
