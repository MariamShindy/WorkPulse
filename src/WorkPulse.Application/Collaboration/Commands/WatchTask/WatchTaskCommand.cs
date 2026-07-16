using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Commands.WatchTask;

public sealed record WatchTaskCommand(Guid TaskId) : IRequest<Result>, IBaseRequest, ICommand;
