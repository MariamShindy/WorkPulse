using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.AddTeamMember;

public sealed record AddTeamMemberCommand(Guid TeamId, Guid UserId, TeamMemberRole Role) : IRequest<Result<TeamMemberDto>>, IBaseRequest, ICommand;
