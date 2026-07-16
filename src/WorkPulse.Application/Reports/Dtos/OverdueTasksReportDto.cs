using System;
using System.Collections.Generic;

namespace WorkPulse.Application.Reports.Dtos;

public sealed record OverdueTasksReportDto(DateTime GeneratedAtUtc, int TotalOverdue, IReadOnlyList<ReportTaskRowDto> Rows);
