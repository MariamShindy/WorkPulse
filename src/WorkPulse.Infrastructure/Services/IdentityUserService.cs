using Microsoft.AspNetCore.Identity;

namespace WorkPulse.Infrastructure.Services;

public sealed class IdentityUserService(
	UserManager<ApplicationUser> userManager,
	SignInManager<ApplicationUser> signInManager,
	IDateTime dateTime) : IUserIdentityService
{
	public async Task<Result<UserIdentityDto>> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default(CancellationToken))
	{
		string normalizedEmail = email.Trim().ToLowerInvariant();
		if (await userManager.FindByEmailAsync(normalizedEmail) != null)
		{
			return Error.Conflict("Auth.EmailTaken", "An account with this email already exists.");
		}
		DateTime now = dateTime.UtcNow;
		ApplicationUser user = new ApplicationUser
		{
			Id = Guid.NewGuid(),
			UserName = normalizedEmail,
			Email = normalizedEmail,
			FirstName = firstName.Trim(),
			LastName = lastName.Trim(),
			CreatedAtUtc = now,
			IsActive = true,
			EmailConfirmed = true,
			// Required for the configured 5-attempt lockout to apply; IdentityUser defaults this to false.
			LockoutEnabled = true
		};
		IdentityResult result = await userManager.CreateAsync(user, password);
		if (!result.Succeeded)
		{
			string message = string.Join("; ", result.Errors.Select((IdentityError e) => e.Description));
			return Error.Validation("Auth.RegistrationFailed", message);
		}
		return MapUser(user);
	}

	public async Task<Result<UserIdentityDto>> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default(CancellationToken))
	{
		string normalizedEmail = email.Trim().ToLowerInvariant();
		ApplicationUser? user = await userManager.FindByEmailAsync(normalizedEmail);
		if (user == null)
		{
			return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
		}
		// CheckPasswordSignInAsync (unlike CheckPasswordAsync) increments AccessFailedCount and
		// honours LockoutEnd, which is what makes the configured lockout policy take effect.
		SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

		if (signInResult.IsLockedOut)
		{
			return Error.Unauthorized(
				"Auth.AccountLocked",
				"Too many failed sign-in attempts. Please try again later.");
		}

		if (!signInResult.Succeeded)
		{
			return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
		}

		return MapUser(user);
	}

	public async Task<UserIdentityDto?> GetByIdAsync(Guid userId, CancellationToken ct = default(CancellationToken))
	{
		ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
		return (user == null) ? null : MapUser(user);
	}

	public async Task<UserIdentityDto?> GetByEmailAsync(string email, CancellationToken ct = default(CancellationToken))
	{
		ApplicationUser? user = await userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());
		return (user == null) ? null : MapUser(user);
	}

	public async Task<IReadOnlyDictionary<Guid, UserIdentityDto>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default(CancellationToken))
	{
		Guid[] ids = userIds.Distinct().ToArray();
		if (ids.Length == 0)
		{
			return new Dictionary<Guid, UserIdentityDto>();
		}

		List<ApplicationUser> users = await userManager.Users
			.AsNoTracking()
			.Where((ApplicationUser u) => ids.Contains(u.Id))
			.ToListAsync(ct);

		return users.ToDictionary((ApplicationUser u) => u.Id, MapUser);
	}

	public async Task<Result<UserIdentityDto>> UpdateProfileAsync(Guid userId, string firstName, string lastName, string? avatarUrl, CancellationToken ct = default(CancellationToken))
	{
		ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
		if (user == null)
		{
			return Error.NotFound("Auth.UserNotFound", "User not found.");
		}
		user.FirstName = firstName.Trim();
		user.LastName = lastName.Trim();
		user.AvatarUrl = avatarUrl?.Trim();
		IdentityResult result = await userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			string message = string.Join("; ", result.Errors.Select((IdentityError e) => e.Description));
			return Error.Validation("Auth.RegistrationFailed", message);
		}
		return MapUser(user);
	}

	public async Task UpdateLastLoginAsync(Guid userId, CancellationToken ct = default(CancellationToken))
	{
		ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
		if (user != null)
		{
			user.LastLoginAtUtc = dateTime.UtcNow;
			await userManager.UpdateAsync(user);
		}
	}

	public async Task SetCurrentTenantAsync(Guid userId, Guid tenantId, CancellationToken ct = default(CancellationToken))
	{
		ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
		if (user != null)
		{
			user.CurrentTenantId = tenantId;
			await userManager.UpdateAsync(user);
		}
	}

	private static UserIdentityDto MapUser(ApplicationUser user)
	{
		return new UserIdentityDto(user.Id, user.Email ?? string.Empty, user.FirstName, user.LastName, user.AvatarUrl, user.CurrentTenantId, user.CreatedAtUtc, user.LastLoginAtUtc, user.IsActive);
	}
}
