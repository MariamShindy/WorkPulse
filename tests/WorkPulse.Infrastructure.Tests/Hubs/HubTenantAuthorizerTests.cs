using Microsoft.EntityFrameworkCore;
using WorkPulse.API.Hubs;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Tests.TestSupport;

namespace WorkPulse.Infrastructure.Tests.Hubs;

/// <summary>
/// Regression cover for the cross-tenant SignalR leak: the hub's JoinTeam/JoinTask methods used
/// to add any caller to any group, and TenantMembershipMiddleware skips /hubs entirely, so these
/// checks are the only thing standing between a caller and another workspace's realtime events.
/// </summary>
public sealed class HubTenantAuthorizerTests
{
	private static readonly Guid TenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
	private static readonly Guid TenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");
	private static readonly Guid UserInA = Guid.Parse("33333333-3333-3333-3333-333333333333");
	private static readonly Guid Outsider = Guid.Parse("44444444-4444-4444-4444-444444444444");

	private static async Task<ApplicationDbContext> SeedAsync()
	{
		ApplicationDbContext context = TestDbContextFactory.Create(UserInA, TenantA);

		context.CompanyMembers.Add(new CompanyMember
		{
			Id = Guid.NewGuid(),
			TenantId = TenantA,
			UserId = UserInA,
			Role = CompanyMemberRole.Member,
			IsActive = true
		});

		// A team and a task that belong to the *other* tenant.
		context.Teams.Add(new Team { Id = Guid.NewGuid(), TenantId = TenantB, Name = "B Team", Key = "BT" });
		context.TaskItems.Add(new TaskItem { Id = Guid.NewGuid(), TenantId = TenantB, Title = "B secret" });

		// And equivalents inside the caller's own tenant.
		context.Teams.Add(new Team { Id = TeamInA, TenantId = TenantA, Name = "A Team", Key = "AT" });
		context.TaskItems.Add(new TaskItem { Id = TaskInA, TenantId = TenantA, Title = "A task" });

		await context.SaveChangesAsync();
		return context;
	}

	private static readonly Guid TeamInA = Guid.Parse("55555555-5555-5555-5555-555555555555");
	private static readonly Guid TaskInA = Guid.Parse("66666666-6666-6666-6666-666666666666");

	[Fact]
	public async Task IsMemberOfTenant_is_true_for_an_active_member()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var authorizer = new HubTenantAuthorizer(context);

		Assert.True(await authorizer.IsMemberOfTenantAsync(UserInA, TenantA));
	}

	[Fact]
	public async Task IsMemberOfTenant_is_false_for_a_non_member()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var authorizer = new HubTenantAuthorizer(context);

		Assert.False(await authorizer.IsMemberOfTenantAsync(Outsider, TenantA));
	}

	[Fact]
	public async Task IsMemberOfTenant_is_false_when_naming_a_tenant_the_user_does_not_belong_to()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var authorizer = new HubTenantAuthorizer(context);

		// This is the exact header-spoofing case: a real user naming someone else's tenant.
		Assert.False(await authorizer.IsMemberOfTenantAsync(UserInA, TenantB));
	}

	[Fact]
	public async Task IsMemberOfTenant_is_false_for_a_deactivated_member()
	{
		await using ApplicationDbContext context = await SeedAsync();
		CompanyMember member = context.CompanyMembers.Single(m => m.UserId == UserInA);
		member.IsActive = false;
		await context.SaveChangesAsync();

		var authorizer = new HubTenantAuthorizer(context);

		Assert.False(await authorizer.IsMemberOfTenantAsync(UserInA, TenantA));
	}

	[Fact]
	public async Task CanAccessTask_allows_a_task_in_the_callers_own_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var authorizer = new HubTenantAuthorizer(context);

		Assert.True(await authorizer.CanAccessTaskAsync(UserInA, TenantA, TaskInA));
	}

	[Fact]
	public async Task CanAccessTask_denies_a_task_belonging_to_another_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		Guid foreignTaskId = context.TaskItems.IgnoreQueryFilters().Single(t => t.TenantId == TenantB).Id;

		var authorizer = new HubTenantAuthorizer(context);

		// Even though the caller is a legitimate member of TenantA, the task is not theirs.
		Assert.False(await authorizer.CanAccessTaskAsync(UserInA, TenantA, foreignTaskId));
	}

	[Fact]
	public async Task CanAccessTeam_allows_a_team_in_the_callers_own_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		var authorizer = new HubTenantAuthorizer(context);

		Assert.True(await authorizer.CanAccessTeamAsync(UserInA, TenantA, TeamInA));
	}

	[Fact]
	public async Task CanAccessTeam_denies_a_team_belonging_to_another_tenant()
	{
		await using ApplicationDbContext context = await SeedAsync();
		Guid foreignTeamId = context.Teams.IgnoreQueryFilters().Single(t => t.TenantId == TenantB).Id;

		var authorizer = new HubTenantAuthorizer(context);

		Assert.False(await authorizer.CanAccessTeamAsync(UserInA, TenantA, foreignTeamId));
	}

	[Fact]
	public async Task CanAccessTask_denies_a_soft_deleted_task()
	{
		await using ApplicationDbContext context = await SeedAsync();
		TaskItem task = context.TaskItems.IgnoreQueryFilters().Single(t => t.Id == TaskInA);
		task.IsDeleted = true;
		await context.SaveChangesAsync();

		var authorizer = new HubTenantAuthorizer(context);

		Assert.False(await authorizer.CanAccessTaskAsync(UserInA, TenantA, TaskInA));
	}
}
