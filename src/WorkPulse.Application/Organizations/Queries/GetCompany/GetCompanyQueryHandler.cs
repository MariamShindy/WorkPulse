using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Queries.GetCompany;

public sealed class GetCompanyQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetCompanyQuery, Result<CompanyDto>>
{
	public async Task<Result<CompanyDto>> Handle(GetCompanyQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Company? company = await context.Companies.AsNoTracking().FirstOrDefaultAsync((Company c) => c.Id == tenantContext.TenantId, ct);
		if (company is null)
		{
			return Error.NotFound("Company.NotFound", "Company not found.");
		}
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
