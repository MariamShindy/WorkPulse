using WorkPulse.Domain.Repositories;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);
}
