namespace WorkPulse.Application.Abstractions;

public interface ITaskRealtimeNotifier
{
	Task NotifyTaskUpdatedAsync(Guid tenantId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken));

	Task NotifyTaskMovedAsync(Guid tenantId, Guid teamId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken));

	Task NotifyUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default(CancellationToken));

	Task NotifyTeamAsync(Guid teamId, string eventName, object payload, CancellationToken ct = default(CancellationToken));
}
