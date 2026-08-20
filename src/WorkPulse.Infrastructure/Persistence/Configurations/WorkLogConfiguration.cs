using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
	public void Configure(EntityTypeBuilder<WorkLog> builder)
	{
		builder.ToTable("work_logs");
		builder.HasKey((WorkLog w) => w.Id);
		builder.Property((WorkLog w) => w.Hours).HasPrecision(10, 2);
		builder.Property((WorkLog w) => w.Description).HasMaxLength(2000);
		builder.HasIndex((WorkLog w) => w.TaskId);
		builder.HasIndex((WorkLog w) => w.UserId);
		builder.HasIndex((WorkLog w) => new { w.TenantId, w.TaskId, w.LoggedDate });
	}
}
