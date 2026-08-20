namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>Per-member task load. Feeds "who has the most tasks" style questions.</summary>
public sealed record MemberWorkloadDto(
    Guid UserId,
    string Name,
    string Email,
    int OpenTasks,
    int OverdueTasks,
    int CompletedTasks,
    int TotalAssigned);
