using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteLabel;

public sealed record DeleteLabelCommand(Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
