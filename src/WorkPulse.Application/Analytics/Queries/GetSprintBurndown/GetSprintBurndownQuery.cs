using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetSprintBurndown;

public sealed record GetSprintBurndownQuery(Guid SprintId) : IRequest<Result<SprintBurndownDto>>, IBaseRequest, IQuery;
