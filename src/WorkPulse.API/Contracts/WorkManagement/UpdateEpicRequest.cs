using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record UpdateEpicRequest(string Title, string? Description, EpicStatus Status);
