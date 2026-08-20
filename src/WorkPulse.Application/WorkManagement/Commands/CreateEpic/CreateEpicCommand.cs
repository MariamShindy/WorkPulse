using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.CreateEpic;

public sealed record CreateEpicCommand(Guid TeamId, string Title, string? Description, EpicStatus Status) : IRequest<Result<EpicDto>>, IBaseRequest, ICommand;
