using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;

namespace WorkPulse.Application.Collaboration.Commands.AddTaskComment;

public sealed record AddTaskCommentCommand(Guid TaskId, string Body, IReadOnlyList<Guid>? MentionedUserIds) : IRequest<Result<TaskCommentDto>>, IBaseRequest, ICommand;
