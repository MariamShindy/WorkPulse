using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Type).HasMaxLength(512).IsRequired();
        builder.Property(o => o.Content).HasColumnType("jsonb").IsRequired();
        builder.Property(o => o.Error).HasMaxLength(2000);
        builder.HasIndex(o => o.ProcessedOnUtc);
    }
}
