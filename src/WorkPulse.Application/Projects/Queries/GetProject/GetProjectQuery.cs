using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.GetProject;

public sealed record GetProjectQuery(Guid ProjectId) : IRequest<Result<ProjectDto>>, IBaseRequest, IQuery;
