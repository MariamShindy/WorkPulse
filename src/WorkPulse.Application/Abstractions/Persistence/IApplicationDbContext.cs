
namespace WorkPulse.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
	DbSet<Company> Companies { get; }

	DbSet<CompanyMember> CompanyMembers { get; }

	DbSet<Team> Teams { get; }

	DbSet<TeamMember> TeamMembers { get; }

	DbSet<Workflow> Workflows { get; }

	DbSet<WorkflowState> WorkflowStates { get; }

	DbSet<Project> Projects { get; }

	DbSet<TaskItem> TaskItems { get; }

	DbSet<Label> Labels { get; }

	DbSet<TaskLabel> TaskLabels { get; }

	DbSet<Epic> Epics { get; }

	DbSet<Sprint> Sprints { get; }

	DbSet<TaskAssignee> TaskAssignees { get; }

	DbSet<TaskDependency> TaskDependencies { get; }

	DbSet<WorkLog> WorkLogs { get; }

	DbSet<SavedView> SavedViews { get; }

	DbSet<TeamIssueCounter> TeamIssueCounters { get; }

	DbSet<TaskComment> TaskComments { get; }

	DbSet<TaskWatcher> TaskWatchers { get; }

	DbSet<TaskActivity> TaskActivities { get; }

	DbSet<Notification> Notifications { get; }

	DbSet<StoredFile> StoredFiles { get; }

	DbSet<RefreshToken> RefreshTokens { get; }

	DbSet<UserInvitation> UserInvitations { get; }

	DbSet<AutomationRule> AutomationRules { get; }

	DbSet<AuditLogEntry> AuditLogEntries { get; }
}
