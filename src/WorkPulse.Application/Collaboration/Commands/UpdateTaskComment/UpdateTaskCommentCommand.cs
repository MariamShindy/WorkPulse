using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;

namespace WorkPulse.Application.Collaboration.Commands.UpdateTaskComment;

public sealed record UpdateTaskCommentCommand(Guid CommentId, string Body) : IRequest<Result<TaskCommentDto>>, IBaseRequest, ICommand;
