using Microsoft.AspNetCore.Diagnostics;

namespace WorkPulse.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
	{
		logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
		if (1 == 0)
		{
		}
		ProblemDetails problemDetails = ((!(exception is OperationCanceledException)) ? new ProblemDetails
		{
			Status = 500,
			Title = "Internal Server Error",
			Detail = "An unexpected error occurred."
		} : new ProblemDetails
		{
			Status = 408,
			Title = "Request Timeout",
			Detail = "The request was cancelled or timed out."
		});
		if (1 == 0)
		{
		}
		ProblemDetails problemDetails2 = problemDetails;
		problemDetails2.Extensions["correlationId"] = context.Items["X-Correlation-Id"]?.ToString();
		context.Response.StatusCode = problemDetails2.Status ?? 500;
		await context.Response.WriteAsJsonAsync(problemDetails2, ct);
		return true;
	}
}
