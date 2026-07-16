using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.AddTeamMember;

public sealed class AddTeamMemberCommandValidator : AbstractValidator<AddTeamMemberCommand>
{
	public AddTeamMemberCommandValidator()
	{
		RuleFor((AddTeamMemberCommand x) => x.TeamId).NotEmpty();
		RuleFor((AddTeamMemberCommand x) => x.UserId).NotEmpty();
	}
}
