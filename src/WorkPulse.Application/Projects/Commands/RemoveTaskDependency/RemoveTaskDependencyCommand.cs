using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.RemoveTaskDependency;

public sealed record RemoveTaskDependencyCommand(Guid DependencyId) : IRequest<Result>, IBaseRequest, ICommand;
