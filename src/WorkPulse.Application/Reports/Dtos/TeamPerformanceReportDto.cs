using System;
using System.Collections.Generic;

namespace WorkPulse.Application.Reports.Dtos;

public sealed record TeamPerformanceReportDto(DateTime GeneratedAtUtc, IReadOnlyList<TeamPerformanceRowDto> Teams);
