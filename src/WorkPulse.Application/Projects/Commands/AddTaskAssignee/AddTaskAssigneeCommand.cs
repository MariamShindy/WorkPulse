using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.AddTaskAssignee;

public sealed record AddTaskAssigneeCommand(Guid TaskId, Guid UserId) : IRequest<Result>, IBaseRequest, ICommand;
