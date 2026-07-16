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
		Company company = await context.Companies.FirstOrDefaultAsync((Company c) => c.Id == tenantContext.TenantId, ct);
		if (company == null)
		{
			return Error.NotFound("Company.NotFound", "Company not found.");
		}
		company.Name = request.Name.Trim();
		company.Description = request.Description?.Trim();
		company.LogoUrl = request.LogoUrl;
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
