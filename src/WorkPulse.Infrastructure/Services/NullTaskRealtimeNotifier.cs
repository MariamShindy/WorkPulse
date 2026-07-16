using System;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class NullTaskRealtimeNotifier : ITaskRealtimeNotifier
{
	public Task NotifyTaskUpdatedAsync(Guid tenantId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken))
	{
		return Task.CompletedTask;
	}

	public Task NotifyTaskMovedAsync(Guid tenantId, Guid teamId, Guid taskId, object payload, CancellationToken ct = default(CancellationToken))
	{
		return Task.CompletedTask;
	}

	public Task NotifyUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default(CancellationToken))
	{
		return Task.CompletedTask;
	}

	public Task NotifyTeamAsync(Guid teamId, string eventName, object payload, CancellationToken ct = default(CancellationToken))
	{
		return Task.CompletedTask;
	}
}
