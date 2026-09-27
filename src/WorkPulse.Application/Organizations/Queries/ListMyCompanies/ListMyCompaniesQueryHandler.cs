using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.ListMyCompanies;

public sealed class ListMyCompaniesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser) : IRequestHandler<ListMyCompaniesQuery, Result<IReadOnlyList<UserCompanyDto>>>
{
	public async Task<Result<IReadOnlyList<UserCompanyDto>>> Handle(ListMyCompaniesQuery request, CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		Guid userId = currentUser.UserId.Value;
		return await (from member in context.CompanyMembers.IgnoreQueryFilters().AsNoTracking()
			join company in context.Companies.AsNoTracking() on member.TenantId equals company.Id
			where member.UserId == userId && !member.IsDeleted && member.IsActive && company.IsActive
			orderby company.Name
			select new UserCompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, member.Role.ToString())).ToListAsync(ct);
	}
}
