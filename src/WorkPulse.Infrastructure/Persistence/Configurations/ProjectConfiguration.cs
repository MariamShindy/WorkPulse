using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		builder.ToTable("projects");
		builder.HasKey((Project p) => p.Id);
		builder.Property((Project p) => p.Name).HasMaxLength(200).IsRequired();
		builder.Property((Project p) => p.Key).HasMaxLength(10).IsRequired();
		builder.Property((Project p) => p.Description).HasMaxLength(5000);
		builder.Property((Project p) => p.Status).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((Project p) => new { p.TenantId, p.Key }).IsUnique();
		builder.HasIndex((Project p) => p.TeamId);
	}
}
