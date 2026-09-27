using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Collaboration.Queries.ListNotifications;

public sealed class ListNotificationsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<ListNotificationsQuery, Result<PagedList<NotificationDto>>>
{
	public async Task<Result<PagedList<NotificationDto>>> Handle(ListNotificationsQuery request, CancellationToken ct)
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
		IQueryable<Notification> query = context.Notifications.AsNoTracking().ForTenant(tenantContext).Where((Notification n) => n.UserId == currentUser.UserId.Value);
		if (request.UnreadOnly)
		{
			query = query.Where((Notification n) => !n.IsRead);
		}
		query = query.OrderByDescending((Notification n) => n.CreatedAtUtc);
		return new PagedList<NotificationDto>(totalCount: await query.CountAsync(ct), items: await (from n in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new NotificationDto(n.Id, n.Type.ToString(), n.Title, n.Body, n.IsRead, n.RelatedEntityType, n.RelatedEntityId, n.ActorId, n.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
