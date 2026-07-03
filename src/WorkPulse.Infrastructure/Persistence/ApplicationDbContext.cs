using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Domain.Common;
using WorkPulse.Infrastructure.Persistence.Interceptors;
using WorkPulse.Infrastructure.Persistence.Outbox;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ITenantContext tenantContext,
    AuditableEntityInterceptor auditableInterceptor,
    DomainEventOutboxInterceptor outboxInterceptor)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        ApplyGlobalFilters(builder);
        ApplySnakeCaseNaming(builder);
    }

    private static void ApplySnakeCaseNaming(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName is not null)
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));

            foreach (var key in entity.GetKeys())
                key.SetName(ToSnakeCase(key.GetName()!));

            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
        }
    }

    private static string ToSnakeCase(string name) =>
        string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())).ToLower();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .AddInterceptors(auditableInterceptor, outboxInterceptor);
    }

    private void ApplyGlobalFilters(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (typeof(ITenantEntity).IsAssignableFrom(clrType))
            {
                var tenantId = tenantContext.TenantId;
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetTenantFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(clrType);
                method.Invoke(null, [builder, tenantId]);
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(clrType);
                method.Invoke(null, [builder]);
            }
        }
    }

    private static void SetTenantFilter<TEntity>(ModelBuilder builder, Guid tenantId)
        where TEntity : class, ITenantEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => e.TenantId == tenantId);
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
        where TEntity : class, ISoftDeletable
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }
}
