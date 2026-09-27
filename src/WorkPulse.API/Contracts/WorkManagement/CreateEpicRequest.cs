using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record CreateEpicRequest(Guid TeamId, string Title, string? Description, EpicStatus Status);
