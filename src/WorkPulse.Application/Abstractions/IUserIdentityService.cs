using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Abstractions;

public interface IUserIdentityService
{
	Task<Result<UserIdentityDto>> RegisterAsync(string email, string password, string firstName, string lastName, CancellationToken ct = default(CancellationToken));

	Task<Result<UserIdentityDto>> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default(CancellationToken));

	Task<UserIdentityDto?> GetByIdAsync(Guid userId, CancellationToken ct = default(CancellationToken));

	Task<UserIdentityDto?> GetByEmailAsync(string email, CancellationToken ct = default(CancellationToken));

	Task<IReadOnlyDictionary<Guid, UserIdentityDto>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken ct = default(CancellationToken));

	Task<Result<UserIdentityDto>> UpdateProfileAsync(Guid userId, string firstName, string lastName, string? avatarUrl, CancellationToken ct = default(CancellationToken));

	Task UpdateLastLoginAsync(Guid userId, CancellationToken ct = default(CancellationToken));

	Task SetCurrentTenantAsync(Guid userId, Guid tenantId, CancellationToken ct = default(CancellationToken));
}
