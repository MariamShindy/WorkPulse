using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
	public void Configure(EntityTypeBuilder<Workflow> builder)
	{
		builder.ToTable("workflows");
		builder.HasKey((Workflow w) => w.Id);
		builder.Property((Workflow w) => w.Name).HasMaxLength(100).IsRequired();
		builder.HasIndex((Workflow w) => new { w.TeamId, w.IsDefault });
	}
}
