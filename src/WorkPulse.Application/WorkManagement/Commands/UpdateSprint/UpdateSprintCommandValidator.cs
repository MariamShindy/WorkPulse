using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSprint;

public sealed class UpdateSprintCommandValidator : AbstractValidator<UpdateSprintCommand>
{
	public UpdateSprintCommandValidator()
	{
		RuleFor((UpdateSprintCommand x) => x.SprintId).NotEmpty();
		RuleFor((UpdateSprintCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((UpdateSprintCommand x) => x.Goal).MaximumLength(2000);
		RuleFor((UpdateSprintCommand x) => x.EndDate).GreaterThanOrEqualTo<UpdateSprintCommand, DateOnly>((UpdateSprintCommand x) => x.StartDate);
	}
}
