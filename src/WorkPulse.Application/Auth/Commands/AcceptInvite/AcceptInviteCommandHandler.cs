using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Dtos;

namespace WorkPulse.Application.Auth.Commands.AcceptInvite;

public sealed class AcceptInviteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IUserIdentityService userIdentity, IJwtTokenService jwtTokenService, IDateTime dateTime) : IRequestHandler<AcceptInviteCommand, Result<AuthResponseDto>>
{
	public async Task<Result<AuthResponseDto>> Handle(AcceptInviteCommand request, CancellationToken ct)
	{
		UserInvitation? invitation = await context.UserInvitations.IgnoreQueryFilters().FirstOrDefaultAsync((UserInvitation i) => i.Token == request.Token, ct);
		if (invitation is null)
		{
			return Error.NotFound("Invitation.NotFound", "Invitation not found.");
		}
		if (invitation.Status == InvitationStatus.Cancelled)
		{
			return Error.Conflict("Invitation.Cancelled", "Invitation has been cancelled.");
		}
		if (invitation.Status == InvitationStatus.Accepted)
		{
			return Error.Conflict("Invitation.AlreadyAccepted", "Invitation has already been accepted.");
		}
		if (invitation.ExpiresAtUtc <= dateTime.UtcNow || invitation.Status == InvitationStatus.Expired)
		{
			invitation.Status = InvitationStatus.Expired;
			return Error.Validation("Invitation.Expired", "Invitation has expired.");
		}
		UserIdentityDto user;
		if (currentUser.IsAuthenticated && currentUser.UserId.HasValue)
		{
			UserIdentityDto? existing = await userIdentity.GetByIdAsync(currentUser.UserId.Value, ct);
			if (existing is null)
			{
				return Error.NotFound("Auth.UserNotFound", "User not found.");
			}
			if (!string.Equals(existing.Email, invitation.Email, StringComparison.OrdinalIgnoreCase))
			{
				return Error.Forbidden("Invitation.EmailMismatch", "Authenticated user email does not match the invitation.");
			}
			user = existing;
		}
		else
		{
			if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
			{
				return Error.Validation("Invitation.RegistrationRequired", "Password, first name, and last name are required to accept this invitation.");
			}
			UserIdentityDto? existingByEmail = await userIdentity.GetByEmailAsync(invitation.Email, ct);
			if (existingByEmail is not null)
			{
				return Error.Conflict("Auth.EmailTaken", "An account with this email already exists. Please sign in and accept the invitation.");
			}
			Result<UserIdentityDto> registerResult = await userIdentity.RegisterAsync(invitation.Email, request.Password, request.FirstName, request.LastName, ct);
			if (registerResult.IsFailure)
			{
				return registerResult.Error;
			}
			user = registerResult.Value;
		}
		if (await context.CompanyMembers.IgnoreQueryFilters().AnyAsync((CompanyMember m) => m.TenantId == invitation.TenantId && m.UserId == user.Id && m.IsActive && !m.IsDeleted, ct))
		{
			return Error.Conflict("Company.MemberExists", "User is already a member of this company.");
		}
		context.CompanyMembers.Add(new CompanyMember
		{
			Id = Guid.NewGuid(),
			TenantId = invitation.TenantId,
			UserId = user.Id,
			Role = invitation.Role,
			IsActive = true,
			CreatedAtUtc = dateTime.UtcNow
		});
		invitation.Status = InvitationStatus.Accepted;
		invitation.AcceptedAtUtc = dateTime.UtcNow;
		await userIdentity.SetCurrentTenantAsync(user.Id, invitation.TenantId, ct);
		string fullName = (user.FirstName + " " + user.LastName).Trim();
		var (accessToken, expiresAt) = jwtTokenService.GenerateAccessToken(user.Id, user.Email, fullName);
		var (refreshToken, _) = await jwtTokenService.GenerateRefreshTokenAsync(user.Id, ct);
		await userIdentity.UpdateLastLoginAsync(user.Id, ct);
		user = user with
		{
			CurrentTenantId = invitation.TenantId
		};
		return new AuthResponseDto(accessToken, refreshToken, expiresAt, RegisterCommandHandler.ToProfile(user));
	}
}
