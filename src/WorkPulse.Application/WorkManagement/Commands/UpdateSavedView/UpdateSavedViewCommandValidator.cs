using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSavedView;

public sealed class UpdateSavedViewCommandValidator : AbstractValidator<UpdateSavedViewCommand>
{
	public UpdateSavedViewCommandValidator()
	{
		RuleFor((UpdateSavedViewCommand x) => x.SavedViewId).NotEmpty();
		RuleFor((UpdateSavedViewCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((UpdateSavedViewCommand x) => x.FiltersJson).NotEmpty().MaximumLength(10000);
		RuleFor((UpdateSavedViewCommand x) => x.SortJson).NotEmpty().MaximumLength(5000);
	}
}
