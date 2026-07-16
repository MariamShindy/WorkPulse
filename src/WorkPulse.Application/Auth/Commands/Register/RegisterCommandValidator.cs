using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor((RegisterCommand x) => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
		RuleFor((RegisterCommand x) => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
		RuleFor((RegisterCommand x) => x.FirstName).NotEmpty().MaximumLength(100);
		RuleFor((RegisterCommand x) => x.LastName).NotEmpty().MaximumLength(100);
	}
}
