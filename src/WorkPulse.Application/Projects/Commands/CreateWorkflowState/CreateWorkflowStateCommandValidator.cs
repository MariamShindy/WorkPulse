using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.CreateWorkflowState;

public sealed class CreateWorkflowStateCommandValidator : AbstractValidator<CreateWorkflowStateCommand>
{
	public CreateWorkflowStateCommandValidator()
	{
		RuleFor((CreateWorkflowStateCommand x) => x.TeamId).NotEmpty();
		RuleFor((CreateWorkflowStateCommand x) => x.Name).NotEmpty().MaximumLength(100);
		RuleFor((CreateWorkflowStateCommand x) => x.Color).NotEmpty().MaximumLength(20);
		RuleFor((CreateWorkflowStateCommand x) => x.Position).GreaterThanOrEqualTo(0);
	}
}
