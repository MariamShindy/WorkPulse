using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
	public UpdateTaskCommandValidator()
	{
		RuleFor((UpdateTaskCommand x) => x.TaskId).NotEmpty();
		RuleFor((UpdateTaskCommand x) => x.Title).NotEmpty().MaximumLength(500);
		RuleFor((UpdateTaskCommand x) => x.Description).MaximumLength(10000);
		RuleFor((UpdateTaskCommand x) => x.BlockedReason).MaximumLength(2000);
		RuleFor((UpdateTaskCommand x) => x.StoryPoints).GreaterThan(0).When<UpdateTaskCommand, int?>((UpdateTaskCommand x) => x.StoryPoints.HasValue);
		RuleFor((UpdateTaskCommand x) => x.EstimatedHours).GreaterThan(0m).When<UpdateTaskCommand, decimal?>((UpdateTaskCommand x) => x.EstimatedHours.HasValue);
	}
}
