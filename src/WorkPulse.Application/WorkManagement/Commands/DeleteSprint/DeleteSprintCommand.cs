using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSprint;

public sealed record DeleteSprintCommand(Guid SprintId) : IRequest<Result>, IBaseRequest, ICommand;
