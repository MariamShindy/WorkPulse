using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSprint;

public sealed record UpdateSprintCommand(Guid SprintId, string Name, string? Goal, DateOnly StartDate, DateOnly EndDate, SprintStatus Status) : IRequest<Result<SprintDto>>, IBaseRequest, ICommand;
