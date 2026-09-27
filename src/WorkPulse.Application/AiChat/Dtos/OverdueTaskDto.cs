namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>An overdue task with the assignee resolved to a display name.</summary>
public sealed record OverdueTaskDto(
    string Identifier,
    string Title,
    string AssigneeName,
    DateOnly DueDate,
    int DaysOverdue,
    string Priority,
    string Status);
