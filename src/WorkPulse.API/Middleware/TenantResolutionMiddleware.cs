using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Services;

namespace WorkPulse.API.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
	private const string TenantHeader = "X-Tenant-Id";

	private const string TenantSlugHeader = "X-Tenant-Slug";

	public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
	{
		string tenantIdValue = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
		string tenantSlug = context.Request.Headers["X-Tenant-Slug"].FirstOrDefault();
		if (Guid.TryParse(tenantIdValue, out var tenantId))
		{
			tenantContext.Set(tenantId, tenantSlug);
			context.Items[TenantHttpContextKeys.TenantId] = tenantId;
			context.RequestServices.GetRequiredService<ApplicationDbContext>().SetCurrentTenant(tenantId);
		}
		await next(context);
	}
}
