namespace WorkPulse.Application.Abstractions;

public interface ITenantContext
{
    Guid TenantId { get; }
    string? TenantSlug { get; }
    bool IsResolved { get; }
}
