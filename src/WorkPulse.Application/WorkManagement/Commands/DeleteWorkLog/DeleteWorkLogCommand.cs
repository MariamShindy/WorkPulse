using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteWorkLog;

public sealed record DeleteWorkLogCommand(Guid WorkLogId) : IRequest<Result>, IBaseRequest, ICommand;
