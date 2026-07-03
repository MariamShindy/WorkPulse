using FluentValidation;
using MediatR;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any()) return await next(ct);

        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next(ct);

        var errors = failures
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        // Return the first validation error; callers can inspect all via the exception if needed
        return CreateValidationResult<TResponse>(errors);
    }

    private static TResult CreateValidationResult<TResult>(List<Error> errors)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
            return (TResult)(object)Result.Failure(errors.First());

        var resultType = typeof(TResult).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethods()
            .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
            .MakeGenericMethod(resultType);

        return (TResult)failureMethod.Invoke(null, [errors.First()])!;
    }
}
