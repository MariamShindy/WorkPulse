using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
	public void Configure(EntityTypeBuilder<Company> builder)
	{
		builder.ToTable("companies");
		builder.HasKey((Company c) => c.Id);
		builder.Property((Company c) => c.Name).HasMaxLength(200).IsRequired();
		builder.Property((Company c) => c.Slug).HasMaxLength(200).IsRequired();
		builder.HasIndex((Company c) => c.Slug).IsUnique();
		builder.Property((Company c) => c.LogoUrl).HasMaxLength(500);
		builder.Property((Company c) => c.Description).HasMaxLength(2000);
	}
}
