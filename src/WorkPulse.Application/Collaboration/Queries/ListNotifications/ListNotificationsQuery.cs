using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Queries.ListNotifications;

public sealed record ListNotificationsQuery(PaginationParams Pagination, bool UnreadOnly = false) : IRequest<Result<PagedList<NotificationDto>>>, IBaseRequest, IQuery;
