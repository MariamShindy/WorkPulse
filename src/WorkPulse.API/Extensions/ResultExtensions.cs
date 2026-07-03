using Microsoft.AspNetCore.Mvc;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.API.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();

    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();

    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.IsSuccess
            ? onSuccess(result.Value)
            : result.Error.ToHttpResult();

    private static IResult ToHttpResult(this Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(ToProblem(error)),
            ErrorType.Validation => Results.UnprocessableEntity(ToProblem(error)),
            ErrorType.Conflict => Results.Conflict(ToProblem(error)),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.Forbidden => Results.Forbid(),
            _ => Results.Problem(ToProblem(error))
        };

    private static ProblemDetails ToProblem(Error error) => new()
    {
        Title = error.Type.ToString(),
        Detail = error.Description,
        Extensions = { ["code"] = error.Code }
    };
}
