using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskLabelConfiguration : IEntityTypeConfiguration<TaskLabel>
{
	public void Configure(EntityTypeBuilder<TaskLabel> builder)
	{
		builder.ToTable("task_labels");
		builder.HasKey((TaskLabel tl) => tl.Id);
		builder.HasIndex((TaskLabel tl) => new { tl.TaskId, tl.LabelId }).IsUnique();
		builder.HasIndex((TaskLabel tl) => tl.LabelId);
	}
}
