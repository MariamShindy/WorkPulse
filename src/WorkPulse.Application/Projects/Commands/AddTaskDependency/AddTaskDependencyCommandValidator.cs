using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.AddTaskDependency;

public sealed class AddTaskDependencyCommandValidator : AbstractValidator<AddTaskDependencyCommand>
{
	public AddTaskDependencyCommandValidator()
	{
		RuleFor((AddTaskDependencyCommand x) => x.TaskId).NotEmpty();
		RuleFor((AddTaskDependencyCommand x) => x.DependsOnTaskId).NotEmpty();
	}
}
