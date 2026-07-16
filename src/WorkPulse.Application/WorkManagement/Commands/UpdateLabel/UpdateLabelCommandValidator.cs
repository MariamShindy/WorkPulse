using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateLabel;

public sealed class UpdateLabelCommandValidator : AbstractValidator<UpdateLabelCommand>
{
	public UpdateLabelCommandValidator()
	{
		RuleFor((UpdateLabelCommand x) => x.LabelId).NotEmpty();
		RuleFor((UpdateLabelCommand x) => x.Name).NotEmpty().MaximumLength(100);
		RuleFor((UpdateLabelCommand x) => x.Color).NotEmpty().MaximumLength(20);
	}
}
