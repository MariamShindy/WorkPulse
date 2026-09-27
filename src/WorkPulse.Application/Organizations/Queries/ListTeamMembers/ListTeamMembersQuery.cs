using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListTeamMembers;

public sealed record ListTeamMembersQuery(Guid TeamId, PaginationParams Pagination) : IRequest<Result<PagedList<TeamMemberDto>>>, IBaseRequest, IQuery;
