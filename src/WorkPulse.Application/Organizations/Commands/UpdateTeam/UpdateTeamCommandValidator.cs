using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.UpdateTeam;

public sealed class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
	public UpdateTeamCommandValidator()
	{
		RuleFor((UpdateTeamCommand x) => x.TeamId).NotEmpty();
		RuleFor((UpdateTeamCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((UpdateTeamCommand x) => x.Description).MaximumLength(2000);
	}
}
