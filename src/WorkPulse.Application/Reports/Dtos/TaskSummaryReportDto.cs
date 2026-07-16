using System;
using System.Collections.Generic;

namespace WorkPulse.Application.Reports.Dtos;

public sealed record TaskSummaryReportDto(DateTime GeneratedAtUtc, int TotalTasks, int OpenTasks, int CompletedTasks, int OverdueTasks, IReadOnlyList<ReportTaskRowDto> Rows);
