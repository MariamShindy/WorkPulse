using FluentValidation;

namespace WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;

public sealed class ReorderWorkflowStatesCommandValidator : AbstractValidator<ReorderWorkflowStatesCommand>
{
	public ReorderWorkflowStatesCommandValidator()
	{
		RuleFor((ReorderWorkflowStatesCommand x) => x.TeamId).NotEmpty();
		RuleFor((ReorderWorkflowStatesCommand x) => x.StateIdsInOrder).NotEmpty();
	}
}
