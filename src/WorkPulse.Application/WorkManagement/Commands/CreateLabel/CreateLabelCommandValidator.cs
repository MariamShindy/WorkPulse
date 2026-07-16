using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.CreateLabel;

public sealed class CreateLabelCommandValidator : AbstractValidator<CreateLabelCommand>
{
	public CreateLabelCommandValidator()
	{
		RuleFor((CreateLabelCommand x) => x.Name).NotEmpty().MaximumLength(100);
		RuleFor((CreateLabelCommand x) => x.Color).NotEmpty().MaximumLength(20);
	}
}
