using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
	public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
	{
		builder.ToTable("audit_log_entries");
		builder.HasKey((AuditLogEntry a) => a.Id);
		builder.Property((AuditLogEntry a) => a.EntityType).HasMaxLength(200).IsRequired();
		builder.Property((AuditLogEntry a) => a.Action).HasMaxLength(100).IsRequired();
		builder.Property((AuditLogEntry a) => a.Timestamp).IsRequired();
		builder.HasIndex((AuditLogEntry a) => new { a.TenantId, a.Timestamp });
		builder.HasIndex((AuditLogEntry a) => new { a.TenantId, a.EntityType, a.EntityId });
	}
}
