using System;
using System.Collections.Generic;
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

public sealed class ListCompanyMembersQueryHandler(
	IApplicationDbContext context,
	ITenantContext tenantContext,
	IUserIdentityService userIdentity) : IRequestHandler<ListCompanyMembersQuery, Result<PagedList<CompanyMemberDto>>>
{
	public async Task<Result<PagedList<CompanyMemberDto>>> Handle(ListCompanyMembersQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		IQueryable<CompanyMember> query = context.CompanyMembers
			.AsNoTracking()
			.ForTenant(tenantContext)
			.OrderBy((CompanyMember m) => m.CreatedAtUtc);

		int totalCount = await query.CountAsync(ct);
		List<CompanyMember> members = await query
			.Skip(request.Pagination.Skip)
			.Take(request.Pagination.PageSize)
			.ToListAsync(ct);

		IReadOnlyDictionary<Guid, UserIdentityDto> users = await userIdentity.GetByIdsAsync(
			members.Select((CompanyMember m) => m.UserId),
			ct);

		List<CompanyMemberDto> items = members.Select((CompanyMember m) =>
		{
			users.TryGetValue(m.UserId, out UserIdentityDto? user);
			string fullName = user == null
				? string.Empty
				: (user.FirstName + " " + user.LastName).Trim();
			return new CompanyMemberDto(
				m.Id,
				m.UserId,
				user?.Email ?? string.Empty,
				fullName,
				m.Role.ToString(),
				m.IsActive,
				m.CreatedAtUtc);
		}).ToList();

		return new PagedList<CompanyMemberDto>(items, request.Pagination.Page, request.Pagination.PageSize, totalCount);
	}
}
