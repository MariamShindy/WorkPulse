using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace WorkPulse.API.Configuration;

/// <summary>
/// Request rate limits. Without these, <c>/api/auth/login</c> accepts unlimited credential
/// stuffing and <c>/api/ai/chat</c> can be used to exhaust the Ollama backend (each call holds a
/// connection for up to the configured timeout).
/// </summary>
public static class RateLimitingExtensions
{
	public const string AuthPolicy = "auth";
	public const string AiPolicy = "ai";

	public static IServiceCollection AddWorkPulseRateLimiting(this IServiceCollection services)
	{
		services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			// Tell the client when to retry instead of leaving it to guess.
			options.OnRejected = async (context, ct) =>
			{
				if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
				{
					context.HttpContext.Response.Headers.RetryAfter =
						((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
				}

				context.HttpContext.Response.ContentType = "application/problem+json";
				await context.HttpContext.Response.WriteAsJsonAsync(
					new ProblemDetails
					{
						Title = "Too Many Requests",
						Detail = "Rate limit exceeded. Please retry shortly.",
						Status = StatusCodes.Status429TooManyRequests,
						Extensions = { ["code"] = "RateLimit.Exceeded" }
					},
					ct);
			};

			// Credential endpoints: keyed by client IP, deliberately tight.
			options.AddPolicy(AuthPolicy, httpContext => RateLimitPartition.GetFixedWindowLimiter(
				ClientKey(httpContext),
				_ => new FixedWindowRateLimiterOptions
				{
					PermitLimit = 10,
					Window = TimeSpan.FromMinutes(1),
					QueueLimit = 0
				}));

			// AI chat: keyed by user, since cost is per-account rather than per-connection.
			options.AddPolicy(AiPolicy, httpContext => RateLimitPartition.GetFixedWindowLimiter(
				ClientKey(httpContext),
				_ => new FixedWindowRateLimiterOptions
				{
					PermitLimit = 20,
					Window = TimeSpan.FromMinutes(1),
					QueueLimit = 0
				}));

			// Backstop for everything else so no single caller can saturate the API.
			options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
			{
				if (IsExempt(httpContext))
				{
					return RateLimitPartition.GetNoLimiter("exempt");
				}

				return RateLimitPartition.GetFixedWindowLimiter(
					ClientKey(httpContext),
					_ => new FixedWindowRateLimiterOptions
					{
						PermitLimit = 300,
						Window = TimeSpan.FromMinutes(1),
						QueueLimit = 0
					});
			});
		});

		return services;
	}

	/// <summary>
	/// Partition by user when we know who is calling, else by IP. Using the user id keeps one
	/// tenant behind a shared NAT from consuming a colleague's budget.
	/// </summary>
	private static string ClientKey(HttpContext httpContext)
	{
		string? userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		return !string.IsNullOrEmpty(userId)
			? "user:" + userId
			: "ip:" + (httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
	}

	/// <summary>Health probes and realtime traffic must not be throttled.</summary>
	private static bool IsExempt(HttpContext httpContext)
	{
		PathString path = httpContext.Request.Path;
		return path.StartsWithSegments("/health") || path.StartsWithSegments("/hubs");
	}
}
