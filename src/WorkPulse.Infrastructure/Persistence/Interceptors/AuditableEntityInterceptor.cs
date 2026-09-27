using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WorkPulse.Domain.Common;

namespace WorkPulse.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor(ICurrentUserService currentUser, IDateTime dateTime) : SaveChangesInterceptor
{
	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateAuditFields(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default(CancellationToken))
	{
		UpdateAuditFields(eventData.Context);
		return base.SavingChangesAsync(eventData, result, ct);
	}

	private void UpdateAuditFields(DbContext? context)
	{
		if (context == null)
		{
			return;
		}
		DateTime utcNow = dateTime.UtcNow;
		Guid? userId = currentUser.UserId;
		foreach (EntityEntry<IAuditableEntity> item in context.ChangeTracker.Entries<IAuditableEntity>())
		{
			if (item.State == EntityState.Added)
			{
				item.Property("CreatedAtUtc").CurrentValue = utcNow;
				item.Property("CreatedById").CurrentValue = userId;
			}
			EntityState state = item.State;
			if ((uint)(state - 3) <= 1u)
			{
				item.Property("UpdatedAtUtc").CurrentValue = utcNow;
				item.Property("UpdatedById").CurrentValue = userId;
			}
		}
		foreach (EntityEntry<ISoftDeletable> item2 in context.ChangeTracker.Entries<ISoftDeletable>())
		{
			if (item2.State == EntityState.Deleted)
			{
				item2.State = EntityState.Modified;
				item2.Property("IsDeleted").CurrentValue = true;
				item2.Property("DeletedAtUtc").CurrentValue = utcNow;
				item2.Property("DeletedById").CurrentValue = userId;
			}
		}
	}
}
