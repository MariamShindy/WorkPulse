namespace WorkPulse.Application.Common.Result;

public sealed record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
	public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.None);

	public static readonly Error NullValue = new Error("Error.NullValue", "Null value provided");

	public static Error NotFound(string code, string description)
	{
		return new Error(code, description, ErrorType.NotFound);
	}

	public static Error Validation(string code, string description)
	{
		return new Error(code, description, ErrorType.Validation);
	}

	public static Error Conflict(string code, string description)
	{
		return new Error(code, description, ErrorType.Conflict);
	}

	public static Error Unauthorized(string code, string description)
	{
		return new Error(code, description, ErrorType.Unauthorized);
	}

	public static Error Forbidden(string code, string description)
	{
		return new Error(code, description, ErrorType.Forbidden);
	}

	public static Error Failure(string code, string description)
	{
		return new Error(code, description);
	}
}
