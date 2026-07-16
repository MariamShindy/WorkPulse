using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;

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
		Company company = await context.Companies.AsNoTracking().FirstOrDefaultAsync((Company c) => c.Id == tenantContext.TenantId, ct);
		if (company == null)
		{
			return Error.NotFound("Company.NotFound", "Company not found.");
		}
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
