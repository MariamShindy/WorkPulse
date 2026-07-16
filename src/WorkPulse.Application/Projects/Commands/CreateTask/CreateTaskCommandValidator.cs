using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
	public CreateTaskCommandValidator()
	{
		RuleFor((CreateTaskCommand x) => x.TeamId).NotEmpty();
		RuleFor((CreateTaskCommand x) => x.Title).NotEmpty().MaximumLength(500);
		RuleFor((CreateTaskCommand x) => x.Description).MaximumLength(10000);
		RuleFor((CreateTaskCommand x) => x.BlockedReason).MaximumLength(2000);
		RuleFor((CreateTaskCommand x) => x.StoryPoints).GreaterThan(0).When<CreateTaskCommand, int?>((CreateTaskCommand x) => x.StoryPoints.HasValue);
		RuleFor((CreateTaskCommand x) => x.EstimatedHours).GreaterThan(0m).When<CreateTaskCommand, decimal?>((CreateTaskCommand x) => x.EstimatedHours.HasValue);
	}
}
