using WorkPulse.Application.Behaviors;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetEpic;

public sealed record GetEpicQuery(Guid EpicId) : IRequest<Result<EpicDto>>, IBaseRequest, IQuery;
