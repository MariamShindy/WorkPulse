using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkPulse.Domain.Common;
using WorkPulse.Infrastructure.Persistence;

namespace WorkPulse.Infrastructure.Jobs;

public sealed class OutboxProcessorJob(
    ApplicationDbContext context,
    IPublisher publisher,
    ILogger<OutboxProcessorJob> logger)
{
    private const int BatchSize = 20;

    public async Task ProcessAsync(CancellationToken ct = default)
    {
        var messages = await context.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null && m.RetryCount < 5)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.Type);
                if (eventType is null)
                {
                    logger.LogWarning("Unknown outbox message type: {Type}", message.Type);
                    continue;
                }

                var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(message.Content, eventType)!;
                await publisher.Publish(domainEvent, ct);

                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                message.RetryCount++;
                message.Error = ex.Message;
            }
        }

        await context.SaveChangesAsync(ct);
    }
}
