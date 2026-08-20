using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Dtos;

namespace WorkPulse.Application.Auth.Queries.GetProfile;

public sealed class GetProfileQueryHandler(ICurrentUserService currentUser, IUserIdentityService userIdentity) : IRequestHandler<GetProfileQuery, Result<UserProfileDto>>
{
	public async Task<Result<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken ct)
	{
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		UserIdentityDto? user = await userIdentity.GetByIdAsync(currentUser.UserId.Value, ct);
		if (user is null)
		{
			return Error.NotFound("Auth.UserNotFound", "User not found.");
		}
		return RegisterCommandHandler.ToProfile(user);
	}
}
