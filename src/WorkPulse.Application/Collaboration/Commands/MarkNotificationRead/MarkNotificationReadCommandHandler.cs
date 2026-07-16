using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		Notification notification = await context.Notifications.FirstOrDefaultAsync((Notification n) => n.Id == request.NotificationId && n.UserId == currentUser.UserId.Value, ct);
		if (notification == null)
		{
			return Error.NotFound("Collaboration.NotificationNotFound", "Notification not found.");
		}
		notification.IsRead = true;
		return Result.Success();
	}
}
