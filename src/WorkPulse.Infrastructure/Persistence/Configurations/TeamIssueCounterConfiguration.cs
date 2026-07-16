using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TeamIssueCounterConfiguration : IEntityTypeConfiguration<TeamIssueCounter>
{
	public void Configure(EntityTypeBuilder<TeamIssueCounter> builder)
	{
		builder.ToTable("team_issue_counters");
		builder.HasKey((TeamIssueCounter c) => c.TeamId);
		builder.Property((TeamIssueCounter c) => c.LastNumber).IsRequired();
		builder.HasIndex((TeamIssueCounter c) => c.TenantId);
	}
}
