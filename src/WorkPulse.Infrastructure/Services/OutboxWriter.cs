using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Abstractions;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Services;

public sealed class OutboxWriter(ApplicationDbContext db) : IOutboxWriter
{
	public Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken ct = default(CancellationToken)) where TEvent : class
	{
		db.OutboxMessages.Add(new OutboxMessage
		{
			Id = Guid.NewGuid(),
			EventType = typeof(TEvent).FullName,
			Payload = JsonSerializer.Serialize(integrationEvent, typeof(TEvent)),
			CreatedAtUtc = DateTime.UtcNow
		});
		return Task.CompletedTask;
	}
}
