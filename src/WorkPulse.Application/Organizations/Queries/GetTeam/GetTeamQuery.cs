using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.GetTeam;

public sealed record GetTeamQuery(Guid TeamId) : IRequest<Result<TeamDto>>, IBaseRequest, IQuery;
