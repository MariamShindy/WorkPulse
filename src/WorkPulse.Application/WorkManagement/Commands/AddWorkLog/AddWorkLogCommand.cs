using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.AddWorkLog;

public sealed record AddWorkLogCommand(Guid TaskId, decimal Hours, string? Description, DateOnly LoggedDate) : IRequest<Result<WorkLogDto>>, IBaseRequest, ICommand;
