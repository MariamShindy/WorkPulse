using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WorkPulse.Application.Abstractions;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Infrastructure.Persistence.Interceptors;

public sealed class AuditLogInterceptor(ICurrentUserService currentUser, ITenantContext tenantContext, IDateTime dateTime) : SaveChangesInterceptor
{
	private static readonly HashSet<string> AuditedEntityTypes = new HashSet<string> { "TaskItem", "Project", "Team", "AutomationRule" };

	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		AppendAuditEntries(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default(CancellationToken))
	{
		AppendAuditEntries(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void AppendAuditEntries(DbContext? context)
	{
		if (!(context is ApplicationDbContext applicationDbContext) || !tenantContext.IsResolved)
		{
			return;
		}
		List<EntityEntry<TenantEntity>> entries = context.ChangeTracker.Entries<TenantEntity>().ToList();
		foreach (EntityEntry<TenantEntity> item in entries)
		{
			string name = item.Entity.GetType().Name;
			if (!AuditedEntityTypes.Contains(name))
			{
				continue;
			}
			EntityState state = item.State;
			if ((uint)(state - 2) <= 2u)
			{
				EntityState state2 = item.State;
				if (1 == 0)
				{
				}
				string text = state2 switch
				{
					EntityState.Added => "Created", 
					EntityState.Modified => "Updated", 
					EntityState.Deleted => "Deleted", 
					_ => "Changed", 
				};
				if (1 == 0)
				{
				}
				string action = text;
				var dictionary = ((item.State == EntityState.Modified) ? item.Properties.Where((PropertyEntry p) => p.IsModified).ToDictionary((PropertyEntry p) => p.Metadata.Name, (PropertyEntry p) => new
				{
					Old = p.OriginalValue,
					New = p.CurrentValue
				}) : null);
				applicationDbContext.AuditLogEntries.Add(new AuditLogEntry
				{
					Id = Guid.NewGuid(),
					TenantId = tenantContext.TenantId,
					EntityType = name,
					EntityId = item.Entity.Id,
					Action = action,
					UserId = currentUser.UserId,
					ChangesJson = ((dictionary == null) ? null : JsonSerializer.Serialize(dictionary)),
					Timestamp = dateTime.UtcNow
				});
			}
		}
	}
}
