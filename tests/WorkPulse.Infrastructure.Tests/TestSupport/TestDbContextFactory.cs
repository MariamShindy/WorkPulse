using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Infrastructure.Persistence;
using WorkPulse.Infrastructure.Persistence.Interceptors;
using WorkPulse.Infrastructure.Services;

namespace WorkPulse.Infrastructure.Tests.TestSupport;

internal sealed class FakeCurrentUser(Guid? userId) : ICurrentUserService
{
	public Guid? UserId { get; } = userId;
	public string? Email => "test@example.com";
	public string? FullName => "Test User";
	public bool IsAuthenticated => UserId.HasValue;
}

internal sealed class FixedDateTime : IDateTime
{
	public DateTime UtcNow { get; } = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	public DateOnly TodayUtc => DateOnly.FromDateTime(UtcNow);
}

internal static class TestDbContextFactory
{
	/// <summary>
	/// A throwaway in-memory context. Each call gets its own database name so tests stay isolated.
	/// </summary>
	public static ApplicationDbContext Create(Guid? currentUserId = null, Guid? currentTenantId = null)
	{
		DbContextOptions<ApplicationDbContext> options =
			new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase($"wp-tests-{Guid.NewGuid():N}")
				.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
				.Options;

		var tenantContext = new TenantContext();
		if (currentTenantId.HasValue)
		{
			tenantContext.Set(currentTenantId.Value);
		}

		var currentUser = new FakeCurrentUser(currentUserId);
		var clock = new FixedDateTime();

		var context = new ApplicationDbContext(
			options,
			tenantContext,
			new AuditableEntityInterceptor(currentUser, clock),
			new AuditLogInterceptor(currentUser, tenantContext, clock));

		if (currentTenantId.HasValue)
		{
			context.SetCurrentTenant(currentTenantId.Value);
		}

		return context;
	}
}
