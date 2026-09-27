using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Organizations.Commands.ArchiveTeam;

public sealed record ArchiveTeamCommand(Guid TeamId) : IRequest<Result>, IBaseRequest, ICommand;
