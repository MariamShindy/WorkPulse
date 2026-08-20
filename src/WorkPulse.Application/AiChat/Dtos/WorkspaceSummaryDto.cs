namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>High-level workspace totals for an at-a-glance answer.</summary>
public sealed record WorkspaceSummaryDto(
    int TotalTasks,
    int OpenTasks,
    int CompletedTasks,
    int OverdueTasks,
    int UnassignedTasks,
    int MemberCount,
    int TeamCount,
    int ProjectCount);
