using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Authorization;

public sealed class CompanyRoleRequirement(params CompanyMemberRole[] allowedRoles) : IAuthorizationRequirement
{
	public IReadOnlyList<CompanyMemberRole> AllowedRoles { get; } = allowedRoles;
}
