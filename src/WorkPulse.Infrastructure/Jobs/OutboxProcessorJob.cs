using System.Text.Json;
using MediatR;
using WorkPulse.Domain.Common;

namespace WorkPulse.Infrastructure.Jobs;

public sealed class OutboxProcessorJob(
    ApplicationDbContext db,
    IPublisher publisher,
    ILogger<OutboxProcessorJob> logger)
{
    private const int BatchSize = 50;

    public async Task ProcessAsync(CancellationToken ct = default)
    {
        var batch = await db.OutboxMessages
            .Where(m => m.ProcessedAtUtc == null && m.RetryCount < 5)
            .OrderBy(m => m.CreatedAtUtc)
            .Take(BatchSize)
            .ToListAsync(ct);

        if (batch.Count == 0) return;

        foreach (var message in batch)
        {
            try
            {
                var eventType = Type.GetType(message.EventType);
                if (eventType is null)
                {
                    logger.LogWarning(
                        "Unknown outbox message type {EventType} for message {MessageId} — skipping",
                        message.EventType, message.Id);
                    message.ProcessedAtUtc = DateTime.UtcNow;
                    continue;
                }

                var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(message.Payload, eventType)!;
                await publisher.Publish(domainEvent, ct);

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.Error = null;

                logger.LogInformation(
                    "Published outbox message {MessageId} of type {EventType}",
                    message.Id, message.EventType);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
