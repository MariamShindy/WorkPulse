
namespace WorkPulse.Application.Common.Extensions;

public static class TenantExtensions
{
	public static WorkPulse.Application.Common.Result.Result EnsureResolved(this ITenantContext tenantContext)
	{
		if (!tenantContext.IsResolved)
		{
			return Error.Validation("Tenant.Required", "X-Tenant-Id header is required.");
		}
		return WorkPulse.Application.Common.Result.Result.Success();
	}
}
