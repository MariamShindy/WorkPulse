using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
	public UpdateCompanyCommandValidator()
	{
		RuleFor((UpdateCompanyCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((UpdateCompanyCommand x) => x.Description).MaximumLength(2000);
		RuleFor((UpdateCompanyCommand x) => x.LogoUrl).MaximumLength(500);
	}
}
