
namespace WorkPulse.Application.Reports.Dtos;

public sealed record TeamPerformanceReportDto(DateTime GeneratedAtUtc, IReadOnlyList<TeamPerformanceRowDto> Teams);
