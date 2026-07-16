using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WorkPulse.API.Hubs;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.API.Services;

public sealed class RealtimeNotifier(IHubContext<WorkPulseHub> hubContext) : ITaskRealtimeNotifier
{
	public Task NotifyTaskUpdatedAsync(Guid tenantId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken))
	{
		return hubContext.Clients.Group(WorkPulseHub.TaskGroup(taskId)).SendAsync("TaskUpdated", payload, ct);
	}

	public Task NotifyTaskMovedAsync(Guid tenantId, Guid teamId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken))
	{
		return hubContext.Clients.Groups(WorkPulseHub.TaskGroup(taskId), WorkPulseHub.TeamGroup(teamId)).SendAsync("TaskMoved", payload, ct);
	}

	public Task NotifyUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default(CancellationToken))
	{
		return hubContext.Clients.Group(WorkPulseHub.UserGroup(userId)).SendAsync(eventName, payload, ct);
	}

	public Task NotifyTeamAsync(Guid teamId, string eventName, object payload, CancellationToken ct = default(CancellationToken))
	{
		return hubContext.Clients.Group(WorkPulseHub.TeamGroup(teamId)).SendAsync(eventName, payload, ct);
	}
}
