using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
	public void Configure(EntityTypeBuilder<Notification> builder)
	{
		builder.ToTable("notifications");
		builder.HasKey((Notification n) => n.Id);
		builder.Property((Notification n) => n.Type).HasConversion<string>().HasMaxLength(50);
		builder.Property((Notification n) => n.Title).HasMaxLength(200).IsRequired();
		builder.Property((Notification n) => n.Body).HasMaxLength(2000).IsRequired();
		builder.Property((Notification n) => n.RelatedEntityType).HasMaxLength(50);
		builder.HasIndex((Notification n) => new { n.UserId, n.IsRead, n.CreatedAtUtc });
		builder.HasIndex((Notification n) => new { n.TenantId, n.UserId });
	}
}
