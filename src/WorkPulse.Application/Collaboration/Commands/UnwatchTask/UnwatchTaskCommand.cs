using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Collaboration.Commands.UnwatchTask;

public sealed record UnwatchTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
