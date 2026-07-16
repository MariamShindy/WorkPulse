using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.RemoveTaskAssignee;

public sealed record RemoveTaskAssigneeCommand(Guid TaskId, Guid UserId) : IRequest<Result>, IBaseRequest, ICommand;
