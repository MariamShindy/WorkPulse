using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WorkPulse.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            OperationCanceledException => new ProblemDetails
            {
                Status = StatusCodes.Status408RequestTimeout,
                Title = "Request Timeout",
                Detail = "The request was cancelled or timed out."
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            }
        };

        problemDetails.Extensions["correlationId"] =
            context.Items["X-Correlation-Id"]?.ToString();

        context.Response.StatusCode = problemDetails.Status!.Value;
        await context.Response.WriteAsJsonAsync(problemDetails, ct);

        return true;
    }
}
