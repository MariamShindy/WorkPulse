using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
	public void Configure(EntityTypeBuilder<Sprint> builder)
	{
		builder.ToTable("sprints");
		builder.HasKey((Sprint s) => s.Id);
		builder.Property((Sprint s) => s.Name).HasMaxLength(200).IsRequired();
		builder.Property((Sprint s) => s.Goal).HasMaxLength(2000);
		builder.Property((Sprint s) => s.Status).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((Sprint s) => s.TeamId);
		builder.HasIndex((Sprint s) => new { s.TenantId, s.TeamId, s.Status });
	}
}
