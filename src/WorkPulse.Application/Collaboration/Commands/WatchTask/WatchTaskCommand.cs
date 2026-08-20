using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Collaboration.Commands.WatchTask;

public sealed record WatchTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
