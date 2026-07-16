using System;

namespace WorkPulse.Application.Reports.Dtos;

public sealed record ReportTaskRowDto(Guid TaskId, string Identifier, string Title, string TeamKey, string? ProjectKey, string Status, string Priority, Guid? AssigneeId, DateOnly? DueDate, DateTime CreatedAtUtc);
