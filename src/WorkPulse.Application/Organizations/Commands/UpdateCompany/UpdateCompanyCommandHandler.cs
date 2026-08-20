using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
{
	public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Company? company = await context.Companies.FirstOrDefaultAsync((Company c) => c.Id == tenantContext.TenantId, ct);
		if (company is null)
		{
			return Error.NotFound("Company.NotFound", "Company not found.");
		}
		company.Name = request.Name.Trim();
		company.Description = request.Description?.Trim();
		company.LogoUrl = request.LogoUrl;
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
