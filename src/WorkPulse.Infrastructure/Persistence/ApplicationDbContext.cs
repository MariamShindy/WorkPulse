using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Domain.Common;
using WorkPulse.Infrastructure.Persistence.Interceptors;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext, AuditableEntityInterceptor auditableInterceptor, AuditLogInterceptor auditLogInterceptor) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>((DbContextOptions)options), IApplicationDbContext
{
	private Guid _currentTenantId;

	public void SetCurrentTenant(Guid tenantId) => _currentTenantId = tenantId;

	public DbSet<Company> Companies => Set<Company>();
	public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
	public DbSet<Team> Teams => Set<Team>();
	public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
	public DbSet<Workflow> Workflows => Set<Workflow>();
	public DbSet<WorkflowState> WorkflowStates => Set<WorkflowState>();
	public DbSet<Project> Projects => Set<Project>();
	public DbSet<TaskItem> TaskItems => Set<TaskItem>();
	public DbSet<Label> Labels => Set<Label>();
	public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
	public DbSet<Epic> Epics => Set<Epic>();
	public DbSet<Sprint> Sprints => Set<Sprint>();
	public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
	public DbSet<TaskDependency> TaskDependencies => Set<TaskDependency>();
	public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
	public DbSet<SavedView> SavedViews => Set<SavedView>();
	public DbSet<TeamIssueCounter> TeamIssueCounters => Set<TeamIssueCounter>();
	public DbSet<TaskComment> TaskComments => Set<TaskComment>();
	public DbSet<TaskWatcher> TaskWatchers => Set<TaskWatcher>();
	public DbSet<TaskActivity> TaskActivities => Set<TaskActivity>();
	public DbSet<Notification> Notifications => Set<Notification>();
	public DbSet<StoredFile> StoredFiles => Set<StoredFile>();
	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();
	public DbSet<AutomationRule> AutomationRules => Set<AutomationRule>();
	public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();
	public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		ApplyGlobalFilters(builder);
		ApplySnakeCaseNaming(builder);
	}

	private static void ApplySnakeCaseNaming(ModelBuilder builder)
	{
		foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
		{
			string? tableName = entityType.GetTableName();
			if (tableName != null)
			{
				entityType.SetTableName(ToSnakeCase(tableName));
			}

			foreach (IMutableProperty property in entityType.GetProperties())
			{
				property.SetColumnName(ToSnakeCase(property.GetColumnName() ?? string.Empty));
			}

			foreach (IMutableKey key in entityType.GetKeys())
			{
				key.SetName(ToSnakeCase(key.GetName() ?? string.Empty));
			}

			foreach (IMutableForeignKey foreignKey in entityType.GetForeignKeys())
			{
				foreignKey.SetConstraintName(ToSnakeCase(foreignKey.GetConstraintName() ?? string.Empty));
			}

			foreach (IMutableIndex index in entityType.GetIndexes())
			{
				index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName() ?? string.Empty));
			}
		}
	}

	private static string ToSnakeCase(string name)
	{
		return string.Concat(name.Select((char c, int i) => (i > 0 && char.IsUpper(c)) ? ("_" + c) : c.ToString())).ToLower();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.AddInterceptors(auditableInterceptor, auditLogInterceptor);
	}

	private void ApplyGlobalFilters(ModelBuilder builder)
	{
		foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
		{
			Type clrType = entityType.ClrType;
			bool isTenant = typeof(ITenantEntity).IsAssignableFrom(clrType);
			bool isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(clrType);

			if (isTenant && isSoftDeletable)
			{
				typeof(ApplicationDbContext)
					.GetMethod(nameof(SetTenantAndSoftDeleteFilter), BindingFlags.Instance | BindingFlags.NonPublic)!
					.MakeGenericMethod(clrType)
					.Invoke(this, [builder]);
			}
			else if (isTenant)
			{
				typeof(ApplicationDbContext)
					.GetMethod(nameof(SetTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic)!
					.MakeGenericMethod(clrType)
					.Invoke(this, [builder]);
			}
			else if (isSoftDeletable)
			{
				typeof(ApplicationDbContext)
					.GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.Static | BindingFlags.NonPublic)!
					.MakeGenericMethod(clrType)
					.Invoke(null, [builder]);
			}
		}
	}

	private void SetTenantAndSoftDeleteFilter<TEntity>(ModelBuilder builder)
		where TEntity : class, ITenantEntity, ISoftDeletable
	{
		// EF replaces prior HasQueryFilter calls — combine both so tenant isolation
		// and soft-delete stay active together.
		builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _currentTenantId && !e.IsDeleted);
	}

	private void SetTenantFilter<TEntity>(ModelBuilder builder) where TEntity : class, ITenantEntity
	{
		builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == _currentTenantId);
	}

	private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : class, ISoftDeletable
	{
		builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
	}
}
