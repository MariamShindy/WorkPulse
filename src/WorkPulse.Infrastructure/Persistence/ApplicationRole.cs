using Microsoft.AspNetCore.Identity;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public Guid? TenantId { get; set; }
}
