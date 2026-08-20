using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class TeamMember : TenantEntity
{
	public Guid TeamId { get; set; }

	public Guid UserId { get; set; }

	public TeamMemberRole Role { get; set; } = TeamMemberRole.Member;
}
