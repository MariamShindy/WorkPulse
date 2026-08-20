using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Dtos;

namespace WorkPulse.Application.Auth.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(ICurrentUserService currentUser, IUserIdentityService userIdentity) : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
	public async Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		Result<UserIdentityDto> updateResult = await userIdentity.UpdateProfileAsync(currentUser.UserId.Value, request.FirstName, request.LastName, request.AvatarUrl, ct);
		if (updateResult.IsFailure)
		{
			return updateResult.Error;
		}
		return RegisterCommandHandler.ToProfile(updateResult.Value);
	}
}
