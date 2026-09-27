using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class EpicConfiguration : IEntityTypeConfiguration<Epic>
{
	public void Configure(EntityTypeBuilder<Epic> builder)
	{
		builder.ToTable("epics");
		builder.HasKey((Epic e) => e.Id);
		builder.Property((Epic e) => e.Title).HasMaxLength(500).IsRequired();
		builder.Property((Epic e) => e.Description).HasMaxLength(10000);
		builder.Property((Epic e) => e.Status).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((Epic e) => e.TeamId);
		builder.HasIndex((Epic e) => new { e.TenantId, e.TeamId, e.Status });
	}
}
