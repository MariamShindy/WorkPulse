using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Organizations;

public sealed record UpdateTeamMemberRequest(TeamMemberRole Role);
