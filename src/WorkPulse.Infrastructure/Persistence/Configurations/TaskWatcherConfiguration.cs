using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskWatcherConfiguration : IEntityTypeConfiguration<TaskWatcher>
{
	public void Configure(EntityTypeBuilder<TaskWatcher> builder)
	{
		builder.ToTable("task_watchers");
		builder.HasKey((TaskWatcher w) => w.Id);
		builder.HasIndex((TaskWatcher w) => new { w.TaskId, w.UserId }).IsUnique();
		builder.HasIndex((TaskWatcher w) => w.UserId);
	}
}
