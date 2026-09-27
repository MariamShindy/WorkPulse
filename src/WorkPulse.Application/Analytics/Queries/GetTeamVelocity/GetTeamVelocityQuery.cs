using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetTeamVelocity;

public sealed record GetTeamVelocityQuery(Guid? TeamId = null, int Weeks = 12) : IRequest<Result<IReadOnlyList<TeamVelocityPointDto>>>, IBaseRequest, IQuery;
