using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
	public void Configure(EntityTypeBuilder<StoredFile> builder)
	{
		builder.ToTable("stored_files");
		builder.HasKey((StoredFile f) => f.Id);
		builder.Property((StoredFile f) => f.FileName).HasMaxLength(255).IsRequired();
		builder.Property((StoredFile f) => f.ContentType).HasMaxLength(100).IsRequired();
		builder.Property((StoredFile f) => f.StorageKey).HasMaxLength(500).IsRequired();
		builder.Property((StoredFile f) => f.EntityType).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((StoredFile f) => new { f.EntityType, f.EntityId, f.CreatedAtUtc });
		builder.HasIndex((StoredFile f) => f.StorageKey).IsUnique();
	}
}
