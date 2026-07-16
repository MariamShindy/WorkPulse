using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSprint;

public sealed record DeleteSprintCommand(Guid SprintId) : IRequest<Result>, IBaseRequest, ICommand;
