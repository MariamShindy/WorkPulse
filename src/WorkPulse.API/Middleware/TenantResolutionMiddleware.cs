using WorkPulse.Infrastructure.Services;

namespace WorkPulse.API.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    private const string TenantHeader = "X-Tenant-Id";
    private const string TenantSlugHeader = "X-Tenant-Slug";

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        var tenantIdValue = context.Request.Headers[TenantHeader].FirstOrDefault();
        var tenantSlug = context.Request.Headers[TenantSlugHeader].FirstOrDefault();

        if (Guid.TryParse(tenantIdValue, out var tenantId))
        {
            tenantContext.Set(tenantId, tenantSlug);
        }

        await next(context);
    }
}
