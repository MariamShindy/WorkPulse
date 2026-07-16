using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
	public LoginCommandValidator()
	{
		RuleFor((LoginCommand x) => x.Email).NotEmpty().EmailAddress();
		RuleFor((LoginCommand x) => x.Password).NotEmpty();
	}
}
