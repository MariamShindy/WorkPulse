using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;

namespace WorkPulse.API.Middleware;

public sealed class TenantMembershipMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(
		HttpContext context,
		ITenantContext tenantContext,
		ICurrentUserService currentUser,
		IApplicationDbContext dbContext)
	{
		if (ShouldSkip(context)
		    || !currentUser.IsAuthenticated
		    || !currentUser.UserId.HasValue
		    || !tenantContext.IsResolved)
		{
			await next(context);
			return;
		}

		bool isMember = await dbContext.CompanyMembers
			.IgnoreQueryFilters()
			.AsNoTracking()
			.AnyAsync(
				m => m.UserId == currentUser.UserId.Value
				     && m.TenantId == tenantContext.TenantId
				     && m.IsActive
				     && !m.IsDeleted,
				context.RequestAborted);

		if (!isMember)
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			context.Response.ContentType = "application/problem+json";
			await context.Response.WriteAsJsonAsync(new ProblemDetails
			{
				Title = "Forbidden",
				Detail = "You are not a member of this workspace.",
				Status = StatusCodes.Status403Forbidden,
				Extensions = { ["code"] = "Tenant.Forbidden" }
			});
			return;
		}

		await next(context);
	}

	private static bool ShouldSkip(HttpContext context)
	{
		PathString path = context.Request.Path;

		if (path.StartsWithSegments("/health")
		    || path.StartsWithSegments("/openapi")
		    || path.StartsWithSegments("/scalar")
		    || path.StartsWithSegments("/hangfire")
		    || path.StartsWithSegments("/hubs"))
		{
			return true;
		}

		if (path.StartsWithSegments("/api/auth/register")
		    || path.StartsWithSegments("/api/auth/login")
		    || path.StartsWithSegments("/api/auth/refresh")
		    || path.StartsWithSegments("/api/auth/accept-invite")
		    || path.StartsWithSegments("/api/invitations/accept"))
		{
			return true;
		}

		if (path.StartsWithSegments("/api/users/me/companies")
		    || path.StartsWithSegments("/api/users/profile")
		    || path.StartsWithSegments("/api/users/me/current-company"))
		{
			return true;
		}

		return path.StartsWithSegments("/api/companies")
		       && HttpMethods.IsPost(context.Request.Method)
		       && string.Equals(path.Value?.TrimEnd('/'), "/api/companies", StringComparison.OrdinalIgnoreCase);
	}
}
