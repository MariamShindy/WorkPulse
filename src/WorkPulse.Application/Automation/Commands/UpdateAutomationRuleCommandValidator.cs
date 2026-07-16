using FluentValidation;

namespace WorkPulse.Application.Automation.Commands;

public sealed class UpdateAutomationRuleCommandValidator : AbstractValidator<UpdateAutomationRuleCommand>
{
	public UpdateAutomationRuleCommandValidator()
	{
		RuleFor((UpdateAutomationRuleCommand x) => x.RuleId).NotEmpty();
		RuleFor((UpdateAutomationRuleCommand x) => x.Name).NotEmpty().MaximumLength(200);
	}
}
