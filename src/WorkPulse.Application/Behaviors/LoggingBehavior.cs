using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		string requestName = typeof(TRequest).Name;
		logger.LogInformation("Handling {RequestName}", requestName);
		TResponse response = await next(ct);
		if (response.IsFailure)
		{
			logger.LogWarning("Request {RequestName} failed: [{ErrorCode}] {ErrorMessage}", requestName, response.Error.Code, response.Error.Description);
		}
		else
		{
			logger.LogInformation("Request {RequestName} completed successfully", requestName);
		}
		return response;
	}
}
