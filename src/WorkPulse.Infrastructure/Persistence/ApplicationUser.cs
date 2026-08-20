using Microsoft.AspNetCore.Identity;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class ApplicationUser : IdentityUser<Guid>
{
	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string? AvatarUrl { get; set; }

	public Guid? CurrentTenantId { get; set; }

	public DateTime CreatedAtUtc { get; set; }

	public DateTime? LastLoginAtUtc { get; set; }

	public bool IsActive { get; set; } = true;
}
