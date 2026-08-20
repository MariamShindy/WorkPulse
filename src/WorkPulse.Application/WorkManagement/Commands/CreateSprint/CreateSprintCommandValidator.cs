using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.CreateSprint;

public sealed class CreateSprintCommandValidator : AbstractValidator<CreateSprintCommand>
{
	public CreateSprintCommandValidator()
	{
		RuleFor((CreateSprintCommand x) => x.TeamId).NotEmpty();
		RuleFor((CreateSprintCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateSprintCommand x) => x.Goal).MaximumLength(2000);
		RuleFor((CreateSprintCommand x) => x.EndDate).GreaterThanOrEqualTo<CreateSprintCommand, DateOnly>((CreateSprintCommand x) => x.StartDate).WithMessage("End date must be on or after start date.");
	}
}
