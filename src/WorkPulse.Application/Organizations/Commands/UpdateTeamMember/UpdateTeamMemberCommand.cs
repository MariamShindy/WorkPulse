using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeamMember;

public sealed record UpdateTeamMemberCommand(Guid TeamId, Guid MemberId, TeamMemberRole Role) : IRequest<Result>, IBaseRequest, ICommand;
