using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.CancelInvitation;

public sealed class CancelInvitationCommandValidator : AbstractValidator<CancelInvitationCommand>
{
	public CancelInvitationCommandValidator()
	{
		RuleFor((CancelInvitationCommand x) => x.InvitationId).NotEmpty();
	}
}
