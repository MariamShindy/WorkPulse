using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace WorkPulse.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
	{
		if (!Enumerable.Any(validators))
		{
			return await next(ct);
		}
		ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
		List<ValidationFailure> failures = (from f in validators.Select((IValidator<TRequest> v) => v.Validate(context)).SelectMany((ValidationResult r) => r.Errors)
			where f != null
			select f).ToList();
		if (failures.Count == 0)
		{
			return await next(ct);
		}
		List<Error> errors = failures.Select((ValidationFailure f) => Error.Validation(f.PropertyName, f.ErrorMessage)).ToList();
		return CreateValidationResult<TResponse>(errors);
	}

	private static TResult CreateValidationResult<TResult>(List<Error> errors) where TResult : Result
	{
		if (typeof(TResult) == typeof(Result))
		{
			return (TResult)Result.Failure(errors.First());
		}
		Type type = typeof(TResult).GetGenericArguments()[0];
		MethodInfo methodInfo = typeof(Result).GetMethods().First((MethodInfo m) => m.Name == "Failure" && m.IsGenericMethod).MakeGenericMethod(type);
		return (TResult)methodInfo.Invoke(null, new object[1] { errors.First() })!;
	}
}
