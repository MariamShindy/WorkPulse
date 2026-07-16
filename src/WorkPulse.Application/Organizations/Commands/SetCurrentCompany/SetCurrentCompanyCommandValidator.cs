using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.SetCurrentCompany;

public sealed class SetCurrentCompanyCommandValidator : AbstractValidator<SetCurrentCompanyCommand>
{
	public SetCurrentCompanyCommandValidator()
	{
		RuleFor((SetCurrentCompanyCommand x) => x.CompanyId).NotEmpty();
	}
}
