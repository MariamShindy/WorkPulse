using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetThroughput;

public sealed record GetThroughputQuery(Guid? TeamId = null, AnalyticsGranularity Granularity = AnalyticsGranularity.Week, int Periods = 12) : IRequest<Result<IReadOnlyList<ThroughputPointDto>>>, IBaseRequest, IQuery;
