using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Organizations.Commands.SetCurrentCompany;

public sealed class SetCurrentCompanyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IUserIdentityService userIdentity) : IRequestHandler<SetCurrentCompanyCommand, Result<CompanyDto>>
{
	public async Task<Result<CompanyDto>> Handle(SetCurrentCompanyCommand request, CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		Guid userId = currentUser.UserId.Value;
		if (!(await context.CompanyMembers.IgnoreQueryFilters().AnyAsync((CompanyMember m) => m.TenantId == request.CompanyId && m.UserId == userId && m.IsActive && !m.IsDeleted, ct)))
		{
			return Error.NotFound("Company.MemberNotFound", "You are not a member of this company.");
		}
		Company company = await context.Companies.AsNoTracking().FirstOrDefaultAsync((Company c) => c.Id == request.CompanyId && c.IsActive, ct);
		if (company == null)
		{
			return Error.NotFound("Company.NotFound", "Company not found.");
		}
		await userIdentity.SetCurrentTenantAsync(userId, company.Id, ct);
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
