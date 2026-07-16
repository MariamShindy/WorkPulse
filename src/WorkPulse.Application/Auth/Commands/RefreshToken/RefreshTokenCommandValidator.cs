using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
	public RefreshTokenCommandValidator()
	{
		RuleFor((RefreshTokenCommand x) => x.RefreshToken).NotEmpty();
	}
}
