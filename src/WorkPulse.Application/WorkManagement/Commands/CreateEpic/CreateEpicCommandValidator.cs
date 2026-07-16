using FluentValidation;

namespace WorkPulse.Application.WorkManagement.Commands.CreateEpic;

public sealed class CreateEpicCommandValidator : AbstractValidator<CreateEpicCommand>
{
	public CreateEpicCommandValidator()
	{
		RuleFor((CreateEpicCommand x) => x.TeamId).NotEmpty();
		RuleFor((CreateEpicCommand x) => x.Title).NotEmpty().MaximumLength(500);
		RuleFor((CreateEpicCommand x) => x.Description).MaximumLength(10000);
	}
}
