
namespace WorkPulse.Application.Collaboration.Services;

public interface ITaskCollaborationService
{
	Task RecordActivityAsync(Guid tenantId, Guid taskId, Guid actorId, ActivityType type, string? summary, object? metadata, CancellationToken ct);

	Task NotifyUsersAsync(Guid tenantId, IEnumerable<Guid> userIds, NotificationType type, string title, string body, Guid? actorId, string relatedEntityType, Guid relatedEntityId, CancellationToken ct);

	Task NotifyWatchersAsync(Guid tenantId, Guid taskId, NotificationType type, string title, string body, Guid actorId, IEnumerable<Guid>? excludeUserIds, CancellationToken ct);
}
