using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class UserInvitation : TenantEntity
{
	public string Email { get; set; } = string.Empty;

	public CompanyMemberRole Role { get; set; } = CompanyMemberRole.Member;

	public string Token { get; set; } = string.Empty;

	public DateTime ExpiresAtUtc { get; set; }

	public DateTime? AcceptedAtUtc { get; set; }

	public Guid InvitedById { get; set; }

	public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
}
