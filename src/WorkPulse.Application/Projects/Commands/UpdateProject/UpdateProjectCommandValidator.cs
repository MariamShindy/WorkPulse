using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
	public UpdateProjectCommandValidator()
	{
		RuleFor((UpdateProjectCommand x) => x.ProjectId).NotEmpty();
		RuleFor((UpdateProjectCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((UpdateProjectCommand x) => x.Description).MaximumLength(5000);
	}
}
