using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeamMember;

public sealed class UpdateTeamMemberCommandValidator : AbstractValidator<UpdateTeamMemberCommand>
{
	public UpdateTeamMemberCommandValidator()
	{
		RuleFor((UpdateTeamMemberCommand x) => x.TeamId).NotEmpty();
		RuleFor((UpdateTeamMemberCommand x) => x.MemberId).NotEmpty();
	}
}
