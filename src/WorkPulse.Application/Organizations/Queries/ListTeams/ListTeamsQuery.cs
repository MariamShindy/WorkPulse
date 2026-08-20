using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListTeams;

public sealed record ListTeamsQuery(PaginationParams Pagination, SortParams? Sort = null, bool IncludeArchived = false) : IRequest<Result<PagedList<TeamDto>>>, IBaseRequest, IQuery;
