using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.CreateTeam;

public sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
	public CreateTeamCommandValidator()
	{
		RuleFor((CreateTeamCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateTeamCommand x) => x.Key).MaximumLength(6);
		RuleFor((CreateTeamCommand x) => x.Description).MaximumLength(2000);
		RuleFor((CreateTeamCommand x) => x.Icon).MaximumLength(50);
		RuleFor((CreateTeamCommand x) => x.Color).MaximumLength(20);
	}
}
