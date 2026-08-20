using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetDashboardAnalytics;

public sealed record GetDashboardAnalyticsQuery(Guid? TeamId = null, DateOnly? From = null, DateOnly? To = null) : IRequest<Result<DashboardAnalyticsDto>>, IBaseRequest, IQuery;
