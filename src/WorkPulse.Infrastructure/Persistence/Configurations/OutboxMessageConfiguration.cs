using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder)
	{
		builder.ToTable("outbox_messages");
		builder.HasKey((OutboxMessage m) => m.Id);
		builder.Property((OutboxMessage m) => m.EventType).HasMaxLength(500).IsRequired();
		builder.Property((OutboxMessage m) => m.Payload).IsRequired();
		builder.Property((OutboxMessage m) => m.Error).HasMaxLength(4000);
		builder.HasIndex((OutboxMessage m) => new { m.ProcessedAtUtc, m.CreatedAtUtc });
	}
}
