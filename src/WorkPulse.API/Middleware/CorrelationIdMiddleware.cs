using Serilog.Context;

namespace WorkPulse.API.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
	private const string CorrelationIdHeader = "X-Correlation-Id";

	public async Task InvokeAsync(HttpContext context)
	{
		string correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
		context.Items["X-Correlation-Id"] = correlationId;
		context.Response.Headers["X-Correlation-Id"] = correlationId;
		using (LogContext.PushProperty("CorrelationId", correlationId))
		{
			await next(context);
		}
	}
}
