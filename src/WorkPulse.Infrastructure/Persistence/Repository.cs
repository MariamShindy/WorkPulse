using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Repositories;

namespace WorkPulse.Infrastructure.Persistence;

public class Repository<TEntity, TId>(ApplicationDbContext context) : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    protected readonly DbSet<TEntity> DbSet = context.Set<TEntity>();

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id), ct);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) =>
        await DbSet.ToListAsync(ct);

    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default) =>
        await DbSet.Where(predicate).ToListAsync(ct);

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default) =>
        DbSet.AnyAsync(predicate, ct);

    public void Add(TEntity entity) => DbSet.Add(entity);
    public void Update(TEntity entity) => DbSet.Update(entity);
    public void Remove(TEntity entity) => DbSet.Remove(entity);
}
