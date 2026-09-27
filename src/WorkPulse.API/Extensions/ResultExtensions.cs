
namespace WorkPulse.API.Extensions;

public static class ResultExtensions
{
	public static IResult ToHttpResult(this Result result)
	{
		return result.IsSuccess ? Results.NoContent() : result.Error.ToHttpResult();
	}

	public static IResult ToHttpResult<T>(this Result<T> result)
	{
		return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToHttpResult();
	}

	public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess)
	{
		return result.IsSuccess ? onSuccess(result.Value) : result.Error.ToHttpResult();
	}

	private static IResult ToHttpResult(this Error error)
	{
		ErrorType type = error.Type;
		if (1 == 0)
		{
		}
		IResult result = type switch
		{
			ErrorType.NotFound => Results.NotFound(ToProblem(error)), 
			ErrorType.Validation => Results.UnprocessableEntity(ToProblem(error)), 
			ErrorType.Conflict => Results.Conflict(ToProblem(error)), 
			ErrorType.Unauthorized => Results.Unauthorized(), 
			ErrorType.Forbidden => Results.Forbid(), 
			_ => Results.Problem(ToProblem(error)), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static ProblemDetails ToProblem(Error error)
	{
		return new ProblemDetails
		{
			Title = error.Type.ToString(),
			Detail = error.Description,
			Extensions = { ["code"] = error.Code }
		};
	}
}
