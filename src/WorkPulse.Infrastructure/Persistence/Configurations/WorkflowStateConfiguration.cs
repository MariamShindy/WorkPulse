using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class WorkflowStateConfiguration : IEntityTypeConfiguration<WorkflowState>
{
	public void Configure(EntityTypeBuilder<WorkflowState> builder)
	{
		builder.ToTable("workflow_states");
		builder.HasKey((WorkflowState s) => s.Id);
		builder.Property((WorkflowState s) => s.Name).HasMaxLength(100).IsRequired();
		builder.Property((WorkflowState s) => s.Type).HasConversion<string>().HasMaxLength(50);
		builder.Property((WorkflowState s) => s.Color).HasMaxLength(20).IsRequired();
		builder.HasIndex((WorkflowState s) => new { s.WorkflowId, s.Position });
	}
}
