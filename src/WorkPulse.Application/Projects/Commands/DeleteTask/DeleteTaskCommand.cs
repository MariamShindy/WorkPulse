using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
