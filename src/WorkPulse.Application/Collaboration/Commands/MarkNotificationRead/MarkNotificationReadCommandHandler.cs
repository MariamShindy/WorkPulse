
namespace WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<MarkNotificationReadCommand, Result>
{
	public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		Notification? notification = await context.Notifications.FirstOrDefaultAsync((Notification n) => n.Id == request.NotificationId && n.UserId == currentUser.UserId.Value, ct);
		if (notification is null)
		{
			return Error.NotFound("Collaboration.NotificationNotFound", "Notification not found.");
		}
		notification.IsRead = true;
		return Result.Success();
	}
}
