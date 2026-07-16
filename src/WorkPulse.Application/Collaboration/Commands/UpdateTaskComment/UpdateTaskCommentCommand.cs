using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Commands.UpdateTaskComment;

public sealed record UpdateTaskCommentCommand(Guid CommentId, string Body) : IRequest<Result<TaskCommentDto>>, IBaseRequest, ICommand;
