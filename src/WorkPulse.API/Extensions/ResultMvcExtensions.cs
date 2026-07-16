using System;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.API.Extensions;

public static class ResultMvcExtensions
{
	public static ActionResult ToActionResult(this Result result)
	{
		if (result.IsSuccess)
		{
			return new NoContentResult();
		}
		return result.Error.ToActionResult();
	}

	public static ActionResult<T> ToActionResult<T>(this Result<T> result)
	{
		if (result.IsSuccess)
		{
			return result.Value;
		}
		return result.Error.ToActionResult<T>();
	}

	public static ActionResult ToCreatedActionResult<T>(this Result<T> result, string routeName, Func<T, object> routeValues)
	{
		if (result.IsSuccess)
		{
			return new CreatedAtRouteResult(routeName, routeValues(result.Value), result.Value);
		}
		return result.Error.ToActionResult();
	}

	public static ActionResult ToErrorActionResult<T>(this Result<T> result)
	{
		if (result.IsSuccess)
		{
			throw new InvalidOperationException("Cannot convert a successful result to an error action result.");
		}
		return result.Error.ToActionResult();
	}

	private static ActionResult ToActionResult(this Error error)
	{
		ErrorType type = error.Type;
		if (1 == 0)
		{
		}
		ActionResult result = type switch
		{
			ErrorType.NotFound => new NotFoundObjectResult(ToProblem(error)), 
			ErrorType.Validation => new UnprocessableEntityObjectResult(ToProblem(error)), 
			ErrorType.Conflict => new ConflictObjectResult(ToProblem(error)), 
			ErrorType.Unauthorized => new UnauthorizedObjectResult(ToProblem(error)), 
			ErrorType.Forbidden => new ForbidResult(), 
			_ => new ObjectResult(ToProblem(error))
			{
				StatusCode = 500
			}, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static ActionResult<T> ToActionResult<T>(this Error error)
	{
		return error.ToActionResult();
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
