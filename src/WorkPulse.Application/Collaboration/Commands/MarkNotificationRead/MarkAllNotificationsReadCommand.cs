using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result>, IBaseRequest, ICommand;
