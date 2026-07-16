using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Domain.Repositories;

namespace WorkPulse.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
	public Task<int> SaveChangesAsync(CancellationToken ct = default(CancellationToken))
	{
		return context.SaveChangesAsync(ct);
	}
}
