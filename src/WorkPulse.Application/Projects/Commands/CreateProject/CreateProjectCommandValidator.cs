using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
	public CreateProjectCommandValidator()
	{
		RuleFor((CreateProjectCommand x) => x.TeamId).NotEmpty();
		RuleFor((CreateProjectCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateProjectCommand x) => x.Key).MaximumLength(10);
		RuleFor((CreateProjectCommand x) => x.Description).MaximumLength(5000);
	}
}
