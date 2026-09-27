using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskActivityConfiguration : IEntityTypeConfiguration<TaskActivity>
{
	public void Configure(EntityTypeBuilder<TaskActivity> builder)
	{
		builder.ToTable("task_activities");
		builder.HasKey((TaskActivity a) => a.Id);
		builder.Property((TaskActivity a) => a.Type).HasConversion<string>().HasMaxLength(50);
		builder.Property((TaskActivity a) => a.Summary).HasMaxLength(500);
		builder.Property((TaskActivity a) => a.MetadataJson).HasColumnType("jsonb");
		builder.HasIndex((TaskActivity a) => new { a.TaskId, a.CreatedAtUtc });
		builder.HasIndex((TaskActivity a) => a.ActorId);
	}
}
