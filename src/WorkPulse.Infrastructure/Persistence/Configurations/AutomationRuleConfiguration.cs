using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class AutomationRuleConfiguration : IEntityTypeConfiguration<AutomationRule>
{
	public void Configure(EntityTypeBuilder<AutomationRule> builder)
	{
		builder.ToTable("automation_rules");
		builder.HasKey((AutomationRule r) => r.Id);
		builder.Property((AutomationRule r) => r.Name).HasMaxLength(200).IsRequired();
		builder.Property((AutomationRule r) => r.TriggerType).HasConversion<string>().HasMaxLength(50);
		builder.Property((AutomationRule r) => r.ActionType).HasConversion<string>().HasMaxLength(50);
		builder.Property((AutomationRule r) => r.TriggerConfigJson).IsRequired();
		builder.Property((AutomationRule r) => r.ActionConfigJson).IsRequired();
		builder.HasIndex((AutomationRule r) => new { r.TenantId, r.IsEnabled });
	}
}
