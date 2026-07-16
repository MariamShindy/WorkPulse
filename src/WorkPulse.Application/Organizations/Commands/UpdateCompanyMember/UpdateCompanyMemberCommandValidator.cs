using FluentValidation;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompanyMember;

public sealed class UpdateCompanyMemberCommandValidator : AbstractValidator<UpdateCompanyMemberCommand>
{
	public UpdateCompanyMemberCommandValidator()
	{
		RuleFor((UpdateCompanyMemberCommand x) => x.MemberId).NotEmpty();
	}
}
