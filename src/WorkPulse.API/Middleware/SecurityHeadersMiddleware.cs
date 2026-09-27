namespace WorkPulse.API.Middleware;

/// <summary>
/// Adds the baseline response security headers. The API is JSON plus file downloads, so the
/// policy can be restrictive: <c>nosniff</c> is what stops a browser re-interpreting an uploaded
/// file as HTML, and the framing headers stop the API being embedded.
/// <para>
/// The strict <c>default-src 'none'</c> CSP is applied only to API responses — the Scalar
/// reference and Hangfire dashboard are real HTML UIs and would break under it.
/// </para>
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
	private static readonly string[] HtmlUiPrefixes = ["/scalar", "/openapi", "/hangfire"];

	public Task InvokeAsync(HttpContext context)
	{
		bool isHtmlUi = HtmlUiPrefixes.Any(prefix => context.Request.Path.StartsWithSegments(prefix));

		// Set on starting so the headers survive handlers that write to the response directly.
		context.Response.OnStarting(static state =>
		{
			var (httpContext, htmlUi) = ((HttpContext, bool))state;
			IHeaderDictionary headers = httpContext.Response.Headers;

			headers["X-Content-Type-Options"] = "nosniff";
			headers["Referrer-Policy"] = "no-referrer";
			headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

			if (!htmlUi)
			{
				headers["X-Frame-Options"] = "DENY";
				headers["Cross-Origin-Resource-Policy"] = "same-site";

				// No inline script or style is ever legitimate in an API response.
				headers["Content-Security-Policy"] =
					"default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";
			}

			return Task.CompletedTask;
		}, (context, isHtmlUi));

		return next(context);
	}
}
