namespace WorkPulse.Application.Abstractions.ReadServices;

public sealed record ReportFilter(Guid? TeamId = null, Guid? ProjectId = null, Guid? AssigneeId = null, DateOnly? From = null, DateOnly? To = null);
