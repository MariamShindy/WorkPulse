using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateEpic;

public sealed record UpdateEpicCommand(Guid EpicId, string Title, string? Description, EpicStatus Status) : IRequest<Result<EpicDto>>, IBaseRequest, ICommand;
