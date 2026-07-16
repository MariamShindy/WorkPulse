using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteEpic;

public sealed record DeleteEpicCommand(Guid EpicId) : IRequest<Result>, IBaseRequest, ICommand;
