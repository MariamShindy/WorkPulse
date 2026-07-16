using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.RevokeToken;

public sealed class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
	public RevokeTokenCommandValidator()
	{
		RuleFor((RevokeTokenCommand x) => x.RefreshToken).NotEmpty();
	}
}
