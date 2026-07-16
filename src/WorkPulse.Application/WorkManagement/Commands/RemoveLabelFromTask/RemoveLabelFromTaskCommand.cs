using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.RemoveLabelFromTask;

public sealed record RemoveLabelFromTaskCommand(Guid TaskId, Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
