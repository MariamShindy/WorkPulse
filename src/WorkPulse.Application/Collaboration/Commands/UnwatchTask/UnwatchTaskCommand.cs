using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Commands.UnwatchTask;

public sealed record UnwatchTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
