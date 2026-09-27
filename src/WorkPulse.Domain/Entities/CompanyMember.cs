using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class CompanyMember : TenantEntity
{
	public Guid UserId { get; set; }

	public CompanyMemberRole Role { get; set; } = CompanyMemberRole.Member;

	public bool IsActive { get; set; } = true;
}
