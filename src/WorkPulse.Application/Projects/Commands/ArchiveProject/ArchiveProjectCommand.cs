using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.ArchiveProject;

public sealed record ArchiveProjectCommand(Guid ProjectId) : IRequest<Result>, IBaseRequest, ICommand;
