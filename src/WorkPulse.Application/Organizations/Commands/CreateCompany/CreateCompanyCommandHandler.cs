using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Commands.CreateCompany;

public sealed class CreateCompanyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IUserIdentityService userIdentity, IDateTime dateTime) : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
	public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken ct)
	{
		string slug;
		string baseSlug = (slug = SlugHelper.ToSlug(request.Name));
		int suffix = 1;
		while (true)
		{
			if (!(await context.Companies.AnyAsync((Company c) => c.Slug == slug, ct)))
			{
				break;
			}
			slug = $"{baseSlug}-{suffix++}";
		}
		DateTime now = dateTime.UtcNow;
		Guid companyId = Guid.NewGuid();
		Company company = new Company
		{
			Id = companyId,
			Name = request.Name.Trim(),
			Slug = slug,
			Description = request.Description?.Trim(),
			LogoUrl = request.LogoUrl,
			IsActive = true,
			CreatedAtUtc = now
		};
		context.Companies.Add(company);
		if (currentUser.IsAuthenticated && currentUser.UserId.HasValue)
		{
			context.CompanyMembers.Add(new CompanyMember
			{
				Id = Guid.NewGuid(),
				TenantId = companyId,
				UserId = currentUser.UserId.Value,
				Role = CompanyMemberRole.Owner,
				IsActive = true
			});
			await userIdentity.SetCurrentTenantAsync(currentUser.UserId.Value, companyId, ct);
		}
		return new CompanyDto(company.Id, company.Name, company.Slug, company.LogoUrl, company.Description, company.IsActive, company.CreatedAtUtc);
	}
}
