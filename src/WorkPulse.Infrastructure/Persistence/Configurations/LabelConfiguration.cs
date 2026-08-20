using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class LabelConfiguration : IEntityTypeConfiguration<Label>
{
	public void Configure(EntityTypeBuilder<Label> builder)
	{
		builder.ToTable("labels");
		builder.HasKey((Label l) => l.Id);
		builder.Property((Label l) => l.Name).HasMaxLength(100).IsRequired();
		builder.Property((Label l) => l.Color).HasMaxLength(20).IsRequired();
		builder.HasIndex((Label l) => new { l.TenantId, l.Name }).IsUnique();
	}
}
