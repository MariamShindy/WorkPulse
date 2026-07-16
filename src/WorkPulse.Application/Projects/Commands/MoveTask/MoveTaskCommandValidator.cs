using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.MoveTask;

public sealed class MoveTaskCommandValidator : AbstractValidator<MoveTaskCommand>
{
	public MoveTaskCommandValidator()
	{
		RuleFor((MoveTaskCommand x) => x.TaskId).NotEmpty();
		RuleFor((MoveTaskCommand x) => x.WorkflowStateId).NotEmpty();
	}
}
