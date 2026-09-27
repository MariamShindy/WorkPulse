using System.Net;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace WorkPulse.API.Authorization;

/// <summary>
/// Gates the Hangfire dashboard. Hangfire treats an empty <c>Authorization</c> collection as
/// "allow everyone", so the dashboard was previously world-readable: anyone could enumerate job
/// arguments and re-queue or delete jobs.
/// <para>
/// In Development only loopback callers are allowed. Anywhere else the caller must be
/// authenticated <i>and</i> satisfy <see cref="AuthorizationPolicies.RequireCompanyAdmin"/>.
/// Because the dashboard is opened directly in a browser it carries no bearer token or
/// <c>X-Tenant-Id</c> header, so that check fails closed by design — expose the dashboard through
/// a trusted network boundary rather than the public internet.
/// </para>
/// </summary>
public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context)
	{
		HttpContext httpContext = context.GetHttpContext();
		IWebHostEnvironment env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();

		if (env.IsDevelopment())
		{
			return IsLoopback(httpContext);
		}

		if (httpContext.User.Identity?.IsAuthenticated != true)
		{
			return false;
		}

		IAuthorizationService authorization = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();

		// Hangfire's filter contract is synchronous; this is a short DB-backed role check.
		AuthorizationResult result = authorization
			.AuthorizeAsync(httpContext.User, httpContext, AuthorizationPolicies.RequireCompanyAdmin)
			.GetAwaiter()
			.GetResult();

		return result.Succeeded;
	}

	private static bool IsLoopback(HttpContext httpContext)
	{
		IPAddress? remote = httpContext.Connection.RemoteIpAddress;
		return remote is not null && IPAddress.IsLoopback(remote);
	}
}
