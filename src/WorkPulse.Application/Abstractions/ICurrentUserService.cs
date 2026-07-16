using System;

namespace WorkPulse.Application.Abstractions;

public interface ICurrentUserService
{
	Guid? UserId { get; }

	string? Email { get; }

	string? FullName { get; }

	bool IsAuthenticated { get; }
}
