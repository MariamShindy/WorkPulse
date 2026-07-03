using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WorkPulse.Application.Abstractions;
using WorkPulse.Domain.Common;

namespace WorkPulse.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(
    ICurrentUserService currentUser,
    IDateTime dateTime) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = dateTime.UtcNow;
        var userId = currentUser.UserId;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditableEntity.CreatedAtUtc)).CurrentValue = now;
                entry.Property(nameof(IAuditableEntity.CreatedById)).CurrentValue = userId;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property(nameof(IAuditableEntity.UpdatedAtUtc)).CurrentValue = now;
                entry.Property(nameof(IAuditableEntity.UpdatedById)).CurrentValue = userId;
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                entry.Property(nameof(ISoftDeletable.DeletedAtUtc)).CurrentValue = now;
                entry.Property(nameof(ISoftDeletable.DeletedById)).CurrentValue = userId;
            }
        }
    }
}
