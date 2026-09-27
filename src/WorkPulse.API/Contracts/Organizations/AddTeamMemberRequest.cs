using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Organizations;

public sealed record AddTeamMemberRequest(Guid UserId, TeamMemberRole Role);
