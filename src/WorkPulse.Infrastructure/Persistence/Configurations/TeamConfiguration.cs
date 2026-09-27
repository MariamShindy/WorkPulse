using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
	public void Configure(EntityTypeBuilder<Team> builder)
	{
		builder.ToTable("teams");
		builder.HasKey((Team t) => t.Id);
		builder.Property((Team t) => t.Name).HasMaxLength(200).IsRequired();
		builder.Property((Team t) => t.Key).HasMaxLength(6).IsRequired();
		builder.Property((Team t) => t.Description).HasMaxLength(2000);
		builder.Property((Team t) => t.Icon).HasMaxLength(50);
		builder.Property((Team t) => t.Color).HasMaxLength(20);
		builder.HasIndex((Team t) => new { t.TenantId, t.Key }).IsUnique();
	}
}
