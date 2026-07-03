using MediatR;
using Microsoft.Extensions.Logging;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Repositories;

namespace WorkPulse.Application.Behaviors;

/// <summary>
/// Wraps commands (not queries) in a database transaction.
/// Queries bypass this behavior because they carry IQuery marker, not ICommand.
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        logger.LogDebug("Beginning transaction for {RequestName}", typeof(TRequest).Name);

        var response = await next(ct);

        if (response.IsSuccess)
            await unitOfWork.SaveChangesAsync(ct);

        return response;
    }
}

public interface ICommand;
public interface IQuery;
