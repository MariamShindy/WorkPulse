using WorkPulse.Application.Abstractions.ReadServices;

namespace WorkPulse.Application.Abstractions;

public interface IExportService
{
	Task<ExportResult> ExportTasksCsvAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken));

	Task<ExportResult> ExportTasksExcelAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken));

	Task<ExportResult> ExportReportPdfAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken));

	Task<ExportResult> ExportCycleTimeCsvAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken));

	Task<ExportResult> ExportAnalyticsSummaryPdfAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken));
}
