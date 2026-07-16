using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.UpdateWorkflowState;

public sealed class UpdateWorkflowStateCommandValidator : AbstractValidator<UpdateWorkflowStateCommand>
{
	public UpdateWorkflowStateCommandValidator()
	{
		RuleFor((UpdateWorkflowStateCommand x) => x.TeamId).NotEmpty();
		RuleFor((UpdateWorkflowStateCommand x) => x.StateId).NotEmpty();
		RuleFor((UpdateWorkflowStateCommand x) => x.Name).NotEmpty().MaximumLength(100);
		RuleFor((UpdateWorkflowStateCommand x) => x.Color).NotEmpty().MaximumLength(20);
	}
}
