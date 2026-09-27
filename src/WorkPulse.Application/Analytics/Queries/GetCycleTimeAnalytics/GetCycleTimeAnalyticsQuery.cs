using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetCycleTimeAnalytics;

public sealed record GetCycleTimeAnalyticsQuery(Guid? TeamId = null, DateOnly? From = null, DateOnly? To = null) : IRequest<Result<CycleTimeAnalyticsDto>>, IBaseRequest, IQuery;
