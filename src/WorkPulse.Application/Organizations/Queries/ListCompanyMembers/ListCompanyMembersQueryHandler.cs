using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Organizations.Queries.ListCompanyMembers;

public sealed class ListCompanyMembersQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListCompanyMembersQuery, Result<PagedList<CompanyMemberDto>>>
{
	public async Task<Result<PagedList<CompanyMemberDto>>> Handle(ListCompanyMembersQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<CompanyMember> query = from m in context.CompanyMembers.AsNoTracking()
			where m.IsActive
			orderby m.CreatedAtUtc
			select m;
		return new PagedList<CompanyMemberDto>(totalCount: await query.CountAsync(ct), items: await (from m in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new CompanyMemberDto(m.Id, m.UserId, string.Empty, string.Empty, m.Role.ToString(), m.IsActive, m.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
