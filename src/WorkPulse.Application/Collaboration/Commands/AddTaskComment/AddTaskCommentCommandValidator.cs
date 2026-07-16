using FluentValidation;

namespace WorkPulse.Application.Collaboration.Commands.AddTaskComment;

public sealed class AddTaskCommentCommandValidator : AbstractValidator<AddTaskCommentCommand>
{
	public AddTaskCommentCommandValidator()
	{
		RuleFor((AddTaskCommentCommand x) => x.TaskId).NotEmpty();
		RuleFor((AddTaskCommentCommand x) => x.Body).NotEmpty().MaximumLength(10000);
	}
}
