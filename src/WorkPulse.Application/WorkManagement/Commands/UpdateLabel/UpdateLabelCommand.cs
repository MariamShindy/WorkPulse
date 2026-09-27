using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateLabel;

public sealed record UpdateLabelCommand(Guid LabelId, string Name, string Color) : IRequest<Result<LabelDto>>, IBaseRequest, ICommand;
