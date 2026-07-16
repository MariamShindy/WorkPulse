using FluentValidation;

namespace WorkPulse.Application.Automation.Commands;

public sealed class CreateAutomationRuleCommandValidator : AbstractValidator<CreateAutomationRuleCommand>
{
	public CreateAutomationRuleCommandValidator()
	{
		RuleFor((CreateAutomationRuleCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateAutomationRuleCommand x) => x.TriggerConfigJson).NotEmpty();
		RuleFor((CreateAutomationRuleCommand x) => x.ActionConfigJson).NotEmpty();
	}
}
