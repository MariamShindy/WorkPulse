using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WorkPulse.API.Hubs;

[Authorize]
public sealed class WorkPulseHub : Hub
{
	private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> TenantOnlineUsers = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>>();

	public static string TenantGroup(Guid tenantId)
	{
		return $"tenant:{tenantId}";
	}

	public static string TeamGroup(Guid teamId)
	{
		return $"team:{teamId}";
	}

	public static string TaskGroup(Guid taskId)
	{
		return $"task:{taskId}";
	}

	public static string UserGroup(Guid userId)
	{
		return $"user:{userId}";
	}

	public override async Task OnConnectedAsync()
	{
		Guid? tenantId = ResolveTenantId();
		Guid? userId = ResolveUserId();
		if (tenantId.HasValue)
		{
			await base.Groups.AddToGroupAsync(base.Context.ConnectionId, TenantGroup(tenantId.Value));
			if (userId.HasValue)
			{
				await base.Groups.AddToGroupAsync(base.Context.ConnectionId, UserGroup(userId.Value));
				MarkOnline(tenantId.Value, userId.Value);
				await base.Clients.Group(TenantGroup(tenantId.Value)).SendAsync("UserOnline", new
				{
					userId = userId.Value,
					tenantId = tenantId.Value
				});
			}
		}
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		Guid? tenantId = ResolveTenantId();
		Guid? userId = ResolveUserId();
		if (tenantId.HasValue && userId.HasValue)
		{
			MarkOffline(tenantId.Value, userId.Value);
			await base.Clients.Group(TenantGroup(tenantId.Value)).SendAsync("UserOffline", new
			{
				userId = userId.Value,
				tenantId = tenantId.Value
			});
		}
		await base.OnDisconnectedAsync(exception);
	}

	public async Task JoinTeam(Guid teamId)
	{
		await base.Groups.AddToGroupAsync(base.Context.ConnectionId, TeamGroup(teamId));
	}

	public async Task LeaveTeam(Guid teamId)
	{
		await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, TeamGroup(teamId));
	}

	public async Task JoinTask(Guid taskId)
	{
		await base.Groups.AddToGroupAsync(base.Context.ConnectionId, TaskGroup(taskId));
	}

	public async Task LeaveTask(Guid taskId)
	{
		await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, TaskGroup(taskId));
	}

	public Task<IReadOnlyList<Guid>> GetOnlineUsers(Guid tenantId)
	{
		if (TenantOnlineUsers.TryGetValue(tenantId, out ConcurrentDictionary<Guid, byte>? value))
		{
			return Task.FromResult((IReadOnlyList<Guid>)value.Keys.ToList());
		}
		return Task.FromResult((IReadOnlyList<Guid>)Array.Empty<Guid>());
	}

	private static void MarkOnline(Guid tenantId, Guid userId)
	{
		TenantOnlineUsers.GetOrAdd(tenantId, (Guid _) => new ConcurrentDictionary<Guid, byte>())[userId] = 0;
	}

	private static void MarkOffline(Guid tenantId, Guid userId)
	{
		if (TenantOnlineUsers.TryGetValue(tenantId, out ConcurrentDictionary<Guid, byte>? value))
		{
			value.TryRemove(userId, out var _);
		}
	}

	private Guid? ResolveTenantId()
	{
		HttpContext? httpContext = base.Context.GetHttpContext();
		string? input = httpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
		Guid result;
		return Guid.TryParse(input, out result) ? new Guid?(result) : ((Guid?)null);
	}

	private Guid? ResolveUserId()
	{
		string? userIdentifier = base.Context.UserIdentifier;
		Guid result;
		return Guid.TryParse(userIdentifier, out result) ? new Guid?(result) : ((Guid?)null);
	}
}
