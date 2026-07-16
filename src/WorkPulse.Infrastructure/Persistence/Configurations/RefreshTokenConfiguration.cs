using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens");
		builder.HasKey((RefreshToken t) => t.Id);
		builder.Property((RefreshToken t) => t.Token).HasMaxLength(256).IsRequired();
		builder.HasIndex((RefreshToken t) => t.Token).IsUnique();
		builder.HasIndex((RefreshToken t) => t.UserId);
	}
}
