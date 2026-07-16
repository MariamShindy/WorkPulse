using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.CreateSavedView;

public sealed class CreateSavedViewCommandValidator : AbstractValidator<CreateSavedViewCommand>
{
	public CreateSavedViewCommandValidator()
	{
		RuleFor((CreateSavedViewCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateSavedViewCommand x) => x.FiltersJson).NotEmpty().MaximumLength(10000);
		RuleFor((CreateSavedViewCommand x) => x.SortJson).NotEmpty().MaximumLength(5000);
	}
}
