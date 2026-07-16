using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListCompanyMembers;

public sealed record ListCompanyMembersQuery(PaginationParams Pagination) : IRequest<Result<PagedList<CompanyMemberDto>>>, IBaseRequest, IQuery;
