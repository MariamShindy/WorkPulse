using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
{
	public void Configure(EntityTypeBuilder<TaskComment> builder)
	{
		builder.ToTable("task_comments");
		builder.HasKey((TaskComment c) => c.Id);
		builder.Property((TaskComment c) => c.Body).HasMaxLength(10000).IsRequired();
		builder.Property((TaskComment c) => c.MentionedUserIds).HasColumnType("jsonb").HasConversion((List<Guid> v) => JsonSerializer.Serialize(v), (string v) => JsonSerializer.Deserialize<List<Guid>>(v) ?? new List<Guid>());
		builder.HasIndex((TaskComment c) => new { c.TaskId, c.CreatedAtUtc });
		builder.HasIndex((TaskComment c) => c.AuthorId);
	}
}
