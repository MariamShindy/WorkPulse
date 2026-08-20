using System.Text.Json;

namespace WorkPulse.Application.Collaboration.Services;

public sealed class TaskCollaborationService(IApplicationDbContext context) : ITaskCollaborationService
{
	public async Task RecordActivityAsync(Guid tenantId, Guid taskId, Guid actorId, ActivityType type, string? summary, object? metadata, CancellationToken ct)
	{
		context.TaskActivities.Add(new TaskActivity
		{
			Id = Guid.NewGuid(),
			TenantId = tenantId,
			TaskId = taskId,
			ActorId = actorId,
			Type = type,
			Summary = summary,
			MetadataJson = ((metadata == null) ? null : JsonSerializer.Serialize(metadata))
		});
		await Task.CompletedTask;
	}

	public async Task NotifyUsersAsync(Guid tenantId, IEnumerable<Guid> userIds, NotificationType type, string title, string body, Guid? actorId, string relatedEntityType, Guid relatedEntityId, CancellationToken ct)
	{
		foreach (Guid userId in userIds.Distinct())
		{
			context.Notifications.Add(new Notification
			{
				Id = Guid.NewGuid(),
				TenantId = tenantId,
				UserId = userId,
				Type = type,
				Title = title,
				Body = body,
				IsRead = false,
				RelatedEntityType = relatedEntityType,
				RelatedEntityId = relatedEntityId,
				ActorId = actorId
			});
		}
		await Task.CompletedTask;
	}

	public async Task NotifyWatchersAsync(Guid tenantId, Guid taskId, NotificationType type, string title, string body, Guid actorId, IEnumerable<Guid>? excludeUserIds, CancellationToken ct)
	{
		HashSet<Guid> excludes = excludeUserIds?.ToHashSet() ?? new HashSet<Guid>();
		excludes.Add(actorId);
		await NotifyUsersAsync(tenantId, await (from w in context.TaskWatchers.AsNoTracking()
			where w.TaskId == taskId && !excludes.Contains(w.UserId)
			select w.UserId).ToListAsync(ct), type, title, body, actorId, "Task", taskId, ct);
	}
}
