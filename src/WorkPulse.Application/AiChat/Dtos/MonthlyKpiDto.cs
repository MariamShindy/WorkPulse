namespace WorkPulse.Application.AiChat.Dtos;

/// <summary>A single month's KPI roll-up. Month is "yyyy-MM".</summary>
public sealed record MonthlyKpiDto(
    string Month,
    int Created,
    int Completed,
    int Overdue,
    double AvgCycleTimeDays);
