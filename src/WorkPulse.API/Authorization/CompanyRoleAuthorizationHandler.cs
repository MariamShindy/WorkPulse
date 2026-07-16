using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Domain.Entities;

namespace WorkPulse.API.Authorization;

public sealed class CompanyRoleAuthorizationHandler(IApplicationDbContext dbContext, ITenantContext tenantContext, ICurrentUserService currentUser) : AuthorizationHandler<CompanyRoleRequirement>
{
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext authContext, CompanyRoleRequirement requirement)
	{
		if (currentUser.IsAuthenticated && currentUser.UserId.HasValue && tenantContext.IsResolved)
		{
			object resource = authContext.Resource;
			CompanyMember member = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(cancellationToken: (resource is HttpContext httpContext) ? httpContext.RequestAborted : CancellationToken.None, source: dbContext.CompanyMembers.AsNoTracking(), predicate: (CompanyMember m) => m.UserId == currentUser.UserId.Value && m.TenantId == tenantContext.TenantId && m.IsActive && !m.IsDeleted);
			if (member != null && requirement.AllowedRoles.Contains(member.Role))
			{
				authContext.Succeed(requirement);
			}
		}
	}
}
