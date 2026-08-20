using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.InviteUser;

public sealed class InviteUserCommandValidator : AbstractValidator<InviteUserCommand>
{
	public InviteUserCommandValidator()
	{
		RuleFor((InviteUserCommand x) => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
		RuleFor((InviteUserCommand x) => x.Role).IsInEnum().Must((CompanyMemberRole r) => r != CompanyMemberRole.Owner).WithMessage("Owner role cannot be assigned via invitation.");
	}
}
