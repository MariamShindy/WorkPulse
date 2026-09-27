
namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record CreateWorkLogRequest(Guid TaskId, decimal Hours, string? Description, DateOnly LoggedDate);
