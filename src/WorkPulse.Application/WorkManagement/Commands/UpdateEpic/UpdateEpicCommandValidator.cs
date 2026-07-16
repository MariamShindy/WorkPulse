using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.UpdateEpic;

public sealed class UpdateEpicCommandValidator : AbstractValidator<UpdateEpicCommand>
{
	public UpdateEpicCommandValidator()
	{
		RuleFor((UpdateEpicCommand x) => x.EpicId).NotEmpty();
		RuleFor((UpdateEpicCommand x) => x.Title).NotEmpty().MaximumLength(500);
		RuleFor((UpdateEpicCommand x) => x.Description).MaximumLength(10000);
	}
}
