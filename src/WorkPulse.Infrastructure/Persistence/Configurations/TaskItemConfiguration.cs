using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
	public void Configure(EntityTypeBuilder<TaskItem> builder)
	{
		builder.ToTable("tasks");
		builder.HasKey((TaskItem t) => t.Id);
		builder.Property((TaskItem t) => t.Title).HasMaxLength(500).IsRequired();
		builder.Property((TaskItem t) => t.Description).HasMaxLength(10000);
		builder.Property((TaskItem t) => t.Priority).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((TaskItem t) => new { t.TeamId, t.Number }).IsUnique();
		builder.HasIndex((TaskItem t) => t.ProjectId);
		builder.HasIndex((TaskItem t) => t.AssigneeId);
		builder.HasIndex((TaskItem t) => t.WorkflowStateId);
		builder.HasIndex((TaskItem t) => t.ParentTaskId);
		builder.HasIndex((TaskItem t) => new { t.TenantId, t.TeamId, t.CreatedAtUtc });
		builder.HasIndex((TaskItem t) => new { t.TenantId, t.AssigneeId });
		builder.HasIndex((TaskItem t) => new { t.TenantId, t.DueDate });
		builder.HasIndex((TaskItem t) => new { t.TenantId, t.ProjectId, t.WorkflowStateId });
		builder.Property((TaskItem t) => t.EstimatedHours).HasPrecision(10, 2);
		builder.Property((TaskItem t) => t.LoggedHours).HasPrecision(10, 2);
		builder.Property((TaskItem t) => t.BlockedReason).HasMaxLength(2000);
		builder.Property((TaskItem t) => t.RowVersion).IsRowVersion();
		builder.HasIndex((TaskItem t) => new { t.TenantId, t.SprintId, t.CompletedAtUtc });
		builder.HasIndex((TaskItem t) => t.EpicId);
		builder.HasIndex((TaskItem t) => t.SprintId);
		builder.HasIndex((TaskItem t) => t.AssignedTeamId);
	}
}
