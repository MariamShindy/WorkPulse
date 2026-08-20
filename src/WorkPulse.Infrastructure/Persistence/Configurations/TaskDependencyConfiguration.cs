using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
	public void Configure(EntityTypeBuilder<TaskDependency> builder)
	{
		builder.ToTable("task_dependencies");
		builder.HasKey((TaskDependency d) => d.Id);
		builder.Property((TaskDependency d) => d.Type).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((TaskDependency d) => new { d.TaskId, d.DependsOnTaskId, d.Type }).IsUnique();
		builder.HasIndex((TaskDependency d) => d.DependsOnTaskId);
	}
}
