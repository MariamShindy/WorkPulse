using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Commands.CreateLabel;

public sealed record CreateLabelCommand(string Name, string Color) : IRequest<Result<LabelDto>>, IBaseRequest, ICommand;
