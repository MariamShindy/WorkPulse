using FluentValidation;

namespace WorkPulse.Application.Auth.Commands.UpdateProfile;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
	public UpdateProfileCommandValidator()
	{
		RuleFor((UpdateProfileCommand x) => x.FirstName).NotEmpty().MaximumLength(100);
		RuleFor((UpdateProfileCommand x) => x.LastName).NotEmpty().MaximumLength(100);
		RuleFor((UpdateProfileCommand x) => x.AvatarUrl).MaximumLength(500);
	}
}
