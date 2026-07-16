using FluentValidation;

namespace WorkPulse.Application.Collaboration.Commands.UpdateTaskComment;

public sealed class UpdateTaskCommentCommandValidator : AbstractValidator<UpdateTaskCommentCommand>
{
	public UpdateTaskCommentCommandValidator()
	{
		RuleFor((UpdateTaskCommentCommand x) => x.CommentId).NotEmpty();
		RuleFor((UpdateTaskCommentCommand x) => x.Body).NotEmpty().MaximumLength(10000);
	}
}
