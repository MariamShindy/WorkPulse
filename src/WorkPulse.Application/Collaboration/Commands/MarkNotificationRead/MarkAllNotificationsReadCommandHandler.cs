using Microsoft.EntityFrameworkCore.Query;

namespace WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;

public sealed class MarkAllNotificationsReadCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
	public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
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
		await context.Notifications.Where((Notification n) => n.UserId == currentUser.UserId.Value && !n.IsRead).ExecuteUpdateAsync(delegate(UpdateSettersBuilder<Notification> s)
		{
			s.SetProperty((Notification n) => n.IsRead, valueExpression: true);
		}, ct);
		return Result.Success();
	}
}
