using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Organizations.Commands.RemoveTeamMember;

public sealed record RemoveTeamMemberCommand(Guid TeamId, Guid MemberId) : IRequest<Result>, IBaseRequest, ICommand;
