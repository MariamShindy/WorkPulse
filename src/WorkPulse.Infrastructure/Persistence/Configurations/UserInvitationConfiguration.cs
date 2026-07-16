using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Configurations;

public sealed class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
{
	public void Configure(EntityTypeBuilder<UserInvitation> builder)
	{
		builder.ToTable("user_invitations");
		builder.HasKey((UserInvitation i) => i.Id);
		builder.Property((UserInvitation i) => i.Email).HasMaxLength(256).IsRequired();
		builder.Property((UserInvitation i) => i.Token).HasMaxLength(256).IsRequired();
		builder.Property((UserInvitation i) => i.Role).HasConversion<string>().HasMaxLength(50);
		builder.Property((UserInvitation i) => i.Status).HasConversion<string>().HasMaxLength(50);
		builder.HasIndex((UserInvitation i) => i.Token).IsUnique();
		builder.HasIndex((UserInvitation i) => new { i.TenantId, i.Email, i.Status });
	}
}
