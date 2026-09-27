using Microsoft.Extensions.Logging;
using WorkPulse.Domain.Repositories;

namespace WorkPulse.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, ICommand where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		logger.LogDebug("Beginning transaction for {RequestName}", typeof(TRequest).Name);
		TResponse response = await next(ct);
		if (response.IsSuccess)
		{
			await unitOfWork.SaveChangesAsync(ct);
		}
		return response;
	}
}
