using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.AddCompanyMember;

public sealed class AddCompanyMemberCommandValidator : AbstractValidator<AddCompanyMemberCommand>
{
	public AddCompanyMemberCommandValidator()
	{
		RuleFor((AddCompanyMemberCommand x) => x.UserId).NotEmpty();
	}
}
