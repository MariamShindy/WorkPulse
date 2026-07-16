using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.AddWorkLog;

public sealed class AddWorkLogCommandValidator : AbstractValidator<AddWorkLogCommand>
{
	public AddWorkLogCommandValidator()
	{
		RuleFor((AddWorkLogCommand x) => x.TaskId).NotEmpty();
		RuleFor((AddWorkLogCommand x) => x.Hours).GreaterThan(0m).LessThanOrEqualTo(24m);
		RuleFor((AddWorkLogCommand x) => x.Description).MaximumLength(2000);
	}
}
