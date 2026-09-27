using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result>, IBaseRequest, ICommand;
