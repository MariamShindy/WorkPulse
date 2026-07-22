using System;

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

/// <summary>A single month's KPI roll-up. Month is "yyyy-MM".</summary>
public sealed record MonthlyKpiDto(
    string Month,
    int Created,
    int Completed,
    int Overdue,
    double AvgCycleTimeDays);

/// <summary>An overdue task with the assignee resolved to a display name.</summary>
public sealed record OverdueTaskDto(
    string Identifier,
    string Title,
    string AssigneeName,
    DateOnly DueDate,
    int DaysOverdue,
    string Priority,
    string Status);

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
