using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class SavedViewConfiguration : IEntityTypeConfiguration<SavedView>
{
	public void Configure(EntityTypeBuilder<SavedView> builder)
	{
		builder.ToTable("saved_views");
		builder.HasKey((SavedView v) => v.Id);
		builder.Property((SavedView v) => v.Name).HasMaxLength(200).IsRequired();
		builder.Property((SavedView v) => v.EntityType).HasConversion<string>().HasMaxLength(50);
		builder.Property((SavedView v) => v.FiltersJson).HasMaxLength(10000).IsRequired();
		builder.Property((SavedView v) => v.SortJson).HasMaxLength(5000).IsRequired();
		builder.HasIndex((SavedView v) => v.UserId);
		builder.HasIndex((SavedView v) => new { v.TenantId, v.UserId, v.EntityType });
	}
}
