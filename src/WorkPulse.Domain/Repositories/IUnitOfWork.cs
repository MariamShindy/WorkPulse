using System.Threading;
using System.Threading.Tasks;

namespace WorkPulse.Domain.Repositories;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken ct = default(CancellationToken));
}
