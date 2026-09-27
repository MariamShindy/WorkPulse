using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
	public void Configure(EntityTypeBuilder<TeamMember> builder)
	{
		builder.ToTable("team_members");
		builder.HasKey((TeamMember m) => m.Id);
		builder.Property((TeamMember m) => m.Role).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((TeamMember m) => new { m.TeamId, m.UserId }).IsUnique();
		builder.HasIndex((TeamMember m) => m.UserId);
	}
}
