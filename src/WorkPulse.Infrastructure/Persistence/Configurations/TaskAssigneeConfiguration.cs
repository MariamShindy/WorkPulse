using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskAssigneeConfiguration : IEntityTypeConfiguration<TaskAssignee>
{
	public void Configure(EntityTypeBuilder<TaskAssignee> builder)
	{
		builder.ToTable("task_assignees");
		builder.HasKey((TaskAssignee a) => a.Id);
		builder.HasIndex((TaskAssignee a) => new { a.TaskId, a.UserId }).IsUnique();
		builder.HasIndex((TaskAssignee a) => a.UserId);
	}
}
