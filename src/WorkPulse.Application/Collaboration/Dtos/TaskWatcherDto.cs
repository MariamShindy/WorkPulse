
namespace WorkPulse.Application.Collaboration.Dtos;

public sealed record TaskWatcherDto(Guid Id, Guid TaskId, Guid UserId, DateTime CreatedAtUtc);
