using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.AssignLabelToTask;

public sealed record AssignLabelToTaskCommand(Guid TaskId, Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
