using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WorkPulse.API.Hubs;

[Authorize]
public sealed class WorkPulseHub(IHubTenantAuthorizer authorizer, ILogger<WorkPulseHub> logger) : Hub
{
	private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, byte>> TenantOnlineUsers = new();

	public static string TenantGroup(Guid tenantId) => $"tenant:{tenantId}";

	public static string TeamGroup(Guid teamId) => $"team:{teamId}";

	public static string TaskGroup(Guid taskId) => $"task:{taskId}";

	public static string UserGroup(Guid userId) => $"user:{userId}";

	public override async Task OnConnectedAsync()
	{
		Guid? tenantId = ResolveTenantId();
		Guid? userId = ResolveUserId();

		// A connection must present a tenant and prove membership of it. Without this the
		// caller could name any tenant in X-Tenant-Id and join that tenant's broadcast group.
		if (!tenantId.HasValue || !userId.HasValue)
		{
			logger.LogWarning("Rejecting hub connection {ConnectionId}: missing tenant or user identity.", Context.ConnectionId);
			Context.Abort();
			return;
		}

		if (!await authorizer.IsMemberOfTenantAsync(userId.Value, tenantId.Value, Context.ConnectionAborted))
		{
			logger.LogWarning(
				"Rejecting hub connection {ConnectionId}: user {UserId} is not a member of tenant {TenantId}.",
				Context.ConnectionId, userId.Value, tenantId.Value);
			Context.Abort();
			return;
		}

		Context.Items[TenantItemKey] = tenantId.Value;
		Context.Items[UserItemKey] = userId.Value;

		await Groups.AddToGroupAsync(Context.ConnectionId, TenantGroup(tenantId.Value));
		await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId.Value));

		MarkOnline(tenantId.Value, userId.Value);
		await Clients.Group(TenantGroup(tenantId.Value)).SendAsync(
			"UserOnline",
			new { userId = userId.Value, tenantId = tenantId.Value });

		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		Guid? tenantId = AuthorizedTenantId();
		Guid? userId = AuthorizedUserId();

		if (tenantId.HasValue && userId.HasValue)
		{
			MarkOffline(tenantId.Value, userId.Value);
			await Clients.Group(TenantGroup(tenantId.Value)).SendAsync(
				"UserOffline",
				new { userId = userId.Value, tenantId = tenantId.Value });
		}

		await base.OnDisconnectedAsync(exception);
	}

	public async Task JoinTeam(Guid teamId)
	{
		var (tenantId, userId) = RequireAuthorizedContext();

		if (!await authorizer.CanAccessTeamAsync(userId, tenantId, teamId, Context.ConnectionAborted))
		{
			throw new HubException("You do not have access to this team.");
		}

		await Groups.AddToGroupAsync(Context.ConnectionId, TeamGroup(teamId));
	}

	public Task LeaveTeam(Guid teamId)
	{
		// Leaving a group it never joined is harmless, so no ownership check is needed here.
		return Groups.RemoveFromGroupAsync(Context.ConnectionId, TeamGroup(teamId));
	}

	public async Task JoinTask(Guid taskId)
	{
		var (tenantId, userId) = RequireAuthorizedContext();

		if (!await authorizer.CanAccessTaskAsync(userId, tenantId, taskId, Context.ConnectionAborted))
		{
			throw new HubException("You do not have access to this task.");
		}

		await Groups.AddToGroupAsync(Context.ConnectionId, TaskGroup(taskId));
	}

	public Task LeaveTask(Guid taskId)
	{
		return Groups.RemoveFromGroupAsync(Context.ConnectionId, TaskGroup(taskId));
	}

	/// <summary>Online users for the caller's own tenant. The parameter is ignored to stop callers probing other tenants.</summary>
	public Task<IReadOnlyList<Guid>> GetOnlineUsers()
	{
		var (tenantId, _) = RequireAuthorizedContext();

		return Task.FromResult(
			TenantOnlineUsers.TryGetValue(tenantId, out ConcurrentDictionary<Guid, byte>? users)
				? (IReadOnlyList<Guid>)users.Keys.ToList()
				: Array.Empty<Guid>());
	}

	private const string TenantItemKey = "wp:tenantId";
	private const string UserItemKey = "wp:userId";

	/// <summary>
	/// The tenant and user proven at connection time. Reading them from <c>Context.Items</c>
	/// rather than re-reading the header means a caller cannot swap tenants mid-connection.
	/// </summary>
	private (Guid TenantId, Guid UserId) RequireAuthorizedContext()
	{
		Guid? tenantId = AuthorizedTenantId();
		Guid? userId = AuthorizedUserId();

		if (!tenantId.HasValue || !userId.HasValue)
		{
			throw new HubException("Connection is not associated with a workspace.");
		}

		return (tenantId.Value, userId.Value);
	}

	private Guid? AuthorizedTenantId() =>
		Context.Items.TryGetValue(TenantItemKey, out object? value) && value is Guid tenantId ? tenantId : null;

	private Guid? AuthorizedUserId() =>
		Context.Items.TryGetValue(UserItemKey, out object? value) && value is Guid userId ? userId : null;

	private static void MarkOnline(Guid tenantId, Guid userId)
	{
		TenantOnlineUsers.GetOrAdd(tenantId, _ => new ConcurrentDictionary<Guid, byte>())[userId] = 0;
	}

	private static void MarkOffline(Guid tenantId, Guid userId)
	{
		if (TenantOnlineUsers.TryGetValue(tenantId, out ConcurrentDictionary<Guid, byte>? users))
		{
			users.TryRemove(userId, out _);
		}
	}

	private Guid? ResolveTenantId()
	{
		HttpContext? httpContext = Context.GetHttpContext();
		string? raw = httpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
		return Guid.TryParse(raw, out Guid tenantId) ? tenantId : null;
	}

	private Guid? ResolveUserId()
	{
		return Guid.TryParse(Context.UserIdentifier, out Guid userId) ? userId : null;
	}
}
