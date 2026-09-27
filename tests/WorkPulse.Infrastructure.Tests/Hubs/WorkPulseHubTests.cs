using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.SignalR;
using WorkPulse.API.Hubs;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Tests.TestSupport;

namespace WorkPulse.Infrastructure.Tests.Hubs;

/// <summary>
/// Drives the hub itself. These are the tests that would have failed before the fix: the old
/// JoinTeam/JoinTask were bare AddToGroupAsync calls with no ownership check, so an authenticated
/// caller could subscribe to any task or team GUID in any workspace.
/// </summary>
public sealed class WorkPulseHubTests
{
	private static readonly Guid TenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
	private static readonly Guid TenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");
	private static readonly Guid UserInA = Guid.Parse("33333333-3333-3333-3333-333333333333");
	private static readonly Guid TaskInA = Guid.Parse("44444444-4444-4444-4444-444444444444");
	private static readonly Guid TaskInB = Guid.Parse("55555555-5555-5555-5555-555555555555");
	private static readonly Guid TeamInB = Guid.Parse("66666666-6666-6666-6666-666666666666");

	private static async Task<ApplicationDbContext> SeedAsync()
	{
		ApplicationDbContext context = TestDbContextFactory.Create(UserInA, TenantA);

		context.CompanyMembers.Add(new CompanyMember
		{
			Id = Guid.NewGuid(), TenantId = TenantA, UserId = UserInA,
			Role = CompanyMemberRole.Member, IsActive = true
		});
		context.TaskItems.Add(new TaskItem { Id = TaskInA, TenantId = TenantA, Title = "A task" });
		context.TaskItems.Add(new TaskItem { Id = TaskInB, TenantId = TenantB, Title = "B secret" });
		context.Teams.Add(new Team { Id = TeamInB, TenantId = TenantB, Name = "B Team", Key = "BT" });

		await context.SaveChangesAsync();
		return context;
	}

	/// <summary>Builds a hub already in the post-connect state for the given tenant.</summary>
	private static (WorkPulseHub Hub, RecordingGroupManager Groups, FakeHubCallerContext Context) BuildHub(
		ApplicationDbContext context, Guid connectedTenantId, Guid connectedUserId)
	{
		var hub = new WorkPulseHub(new HubTenantAuthorizer(context), NullLogger<WorkPulseHub>.Instance);
		var groups = new RecordingGroupManager();
		var callerContext = new FakeHubCallerContext(connectedUserId);

		callerContext.Items["wp:tenantId"] = connectedTenantId;
		callerContext.Items["wp:userId"] = connectedUserId;

		hub.Groups = groups;
		hub.Context = callerContext;

		return (hub, groups, callerContext);
	}

	[Fact]
	public async Task JoinTask_joins_a_task_in_the_callers_own_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, groups, _) = BuildHub(context, TenantA, UserInA);

		await hub.JoinTask(TaskInA);

		Assert.Contains(WorkPulseHub.TaskGroup(TaskInA), groups.Joined);
	}

	[Fact]
	public async Task JoinTask_rejects_a_task_from_another_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, groups, _) = BuildHub(context, TenantA, UserInA);

		await Assert.ThrowsAsync<HubException>(() => hub.JoinTask(TaskInB));

		// The important half: no group was joined, so no events can reach this connection.
		Assert.Empty(groups.Joined);
	}

	[Fact]
	public async Task JoinTeam_rejects_a_team_from_another_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, groups, _) = BuildHub(context, TenantA, UserInA);

		await Assert.ThrowsAsync<HubException>(() => hub.JoinTeam(TeamInB));

		Assert.Empty(groups.Joined);
	}

	[Fact]
	public async Task JoinTask_rejects_a_task_that_does_not_exist()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, groups, _) = BuildHub(context, TenantA, UserInA);

		await Assert.ThrowsAsync<HubException>(() => hub.JoinTask(Guid.NewGuid()));

		Assert.Empty(groups.Joined);
	}

	[Fact]
	public async Task Hub_methods_reject_a_connection_that_never_proved_a_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var hub = new WorkPulseHub(new HubTenantAuthorizer(context), NullLogger<WorkPulseHub>.Instance);
		var groups = new RecordingGroupManager();

		// No Items populated: simulates a caller invoking hub methods without a resolved tenant.
		hub.Groups = groups;
		hub.Context = new FakeHubCallerContext(UserInA);

		await Assert.ThrowsAsync<HubException>(() => hub.JoinTask(TaskInA));
		Assert.Empty(groups.Joined);
	}

	[Fact]
	public async Task JoinTask_cannot_be_widened_by_swapping_the_tenant_header_mid_connection()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, groups, callerContext) = BuildHub(context, TenantA, UserInA);

		// The hub reads the tenant from Items (proven at connect time), never from the header
		// again — so re-sending X-Tenant-Id as TenantB must not grant TenantB's tasks.
		callerContext.Items["X-Tenant-Id"] = TenantB;

		await Assert.ThrowsAsync<HubException>(() => hub.JoinTask(TaskInB));

		Assert.Empty(groups.Joined);
	}

	[Fact]
	public async Task GetOnlineUsers_reports_only_the_callers_own_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var (hub, _, _) = BuildHub(context, TenantA, UserInA);

		IReadOnlyList<Guid> online = await hub.GetOnlineUsers();

		// No connection has been marked online in this test, so the caller's tenant is empty —
		// the point is that the method takes no tenant argument to probe with any more.
		Assert.Empty(online);
	}
}
