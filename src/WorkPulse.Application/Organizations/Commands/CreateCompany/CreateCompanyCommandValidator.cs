using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.CreateCompany;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
	public CreateCompanyCommandValidator()
	{
		RuleFor((CreateCompanyCommand x) => x.Name).NotEmpty().MaximumLength(200);
		RuleFor((CreateCompanyCommand x) => x.Description).MaximumLength(2000);
		RuleFor((CreateCompanyCommand x) => x.LogoUrl).MaximumLength(500);
	}
}
