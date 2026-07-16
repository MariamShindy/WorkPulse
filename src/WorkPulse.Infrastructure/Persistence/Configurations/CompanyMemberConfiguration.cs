using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
	public void Configure(EntityTypeBuilder<CompanyMember> builder)
	{
		builder.ToTable("company_members");
		builder.HasKey((CompanyMember m) => m.Id);
		builder.Property((CompanyMember m) => m.Role).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((CompanyMember m) => new { m.TenantId, m.UserId }).IsUnique();
		builder.HasIndex((CompanyMember m) => m.UserId);
	}
}
