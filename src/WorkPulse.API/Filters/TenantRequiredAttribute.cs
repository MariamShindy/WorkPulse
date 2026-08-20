using Microsoft.AspNetCore.Mvc.Filters;
using WorkPulse.Application.Abstractions;

namespace WorkPulse.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TenantRequiredAttribute : Attribute, IAsyncActionFilter, IFilterMetadata
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		ITenantContext tenantContext = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();
		if (!tenantContext.IsResolved)
		{
			context.Result = new UnprocessableEntityObjectResult(new ProblemDetails
			{
				Title = "Validation",
				Detail = "X-Tenant-Id header is required.",
				Extensions = { ["code"] = "Tenant.Required" }
			});
		}
		else
		{
			await next();
		}
	}
}
