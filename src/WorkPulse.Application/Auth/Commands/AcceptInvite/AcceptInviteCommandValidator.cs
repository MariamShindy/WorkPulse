using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.AcceptInvite;

public sealed class AcceptInviteCommandValidator : AbstractValidator<AcceptInviteCommand>
{
	public AcceptInviteCommandValidator()
	{
		RuleFor((AcceptInviteCommand x) => x.Token).NotEmpty();
		When((AcceptInviteCommand x) => !string.IsNullOrWhiteSpace(x.Password), delegate
		{
			RuleFor((AcceptInviteCommand x) => x.Password).MinimumLength(8).MaximumLength(128);
			RuleFor((AcceptInviteCommand x) => x.FirstName).NotEmpty().MaximumLength(100);
			RuleFor((AcceptInviteCommand x) => x.LastName).NotEmpty().MaximumLength(100);
		});
	}
}
