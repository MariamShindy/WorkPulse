using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetProjectProgress;

public sealed record GetProjectProgressQuery(Guid? TeamId = null) : IRequest<Result<IReadOnlyList<ProjectProgressDto>>>, IBaseRequest, IQuery;
