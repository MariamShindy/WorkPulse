using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result>, IBaseRequest, ICommand;
