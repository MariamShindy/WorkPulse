using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Analytics.Queries.GetAssigneeWorkload;

public sealed record GetAssigneeWorkloadQuery(Guid? TeamId = null) : IRequest<Result<IReadOnlyList<AssigneeWorkloadDto>>>, IBaseRequest, IQuery;
