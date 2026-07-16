using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WorkPulse.Domain.Common;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Persistence.Interceptors;

public sealed class DomainEventOutboxInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = false
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        ConvertDomainEventsToOutboxMessages(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ConvertDomainEventsToOutboxMessages(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private static void ConvertDomainEventsToOutboxMessages(DbContext? context)
    {
        if (context is null) return;

        var outboxMessages = context.ChangeTracker
            .Entries<AggregateRoot<Guid>>()
            .Select(e => e.Entity)
            .SelectMany(aggregate =>
            {
                var events = aggregate.PopDomainEvents();
                return events;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = domainEvent.EventId,
                CreatedAtUtc = domainEvent.OccurredOnUtc,
                EventType = domainEvent.GetType().AssemblyQualifiedName!,
                Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), SerializerOptions)
            })
            .ToList();

        context.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
