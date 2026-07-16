using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Projects.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
