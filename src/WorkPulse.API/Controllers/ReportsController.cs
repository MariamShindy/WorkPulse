using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Reports.Dtos;
using WorkPulse.Application.Reports.Queries;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class ReportsController(ISender sender, IExportService exportService, ITenantContext tenantContext) : ControllerBase
{
	[HttpGet("task-summary")]
	public async Task<ActionResult<TaskSummaryReportDto>> TaskSummary([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<TaskSummaryReportDto>>)new GetTaskSummaryReportQuery(new ReportFilter(teamId, projectId, assigneeId, from, to)), ct)).ToActionResult();
	}

	[HttpGet("overdue-tasks")]
	public async Task<ActionResult<OverdueTasksReportDto>> OverdueTasks([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<OverdueTasksReportDto>>)new GetOverdueTasksReportQuery(new ReportFilter(teamId, projectId, assigneeId)), ct)).ToActionResult();
	}

	[HttpGet("team-performance")]
	public async Task<ActionResult<TeamPerformanceReportDto>> TeamPerformance([FromQuery] Guid? teamId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<TeamPerformanceReportDto>>)new GetTeamPerformanceReportQuery(new ReportFilter(teamId, null, null, from, to)), ct)).ToActionResult();
	}

	[HttpGet("export/tasks/csv")]
	public async Task<IActionResult> ExportTasksCsv([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct = default(CancellationToken))
	{
		ExportResult result = await exportService.ExportTasksCsvAsync(tenantContext.TenantId, new ReportFilter(teamId, projectId, assigneeId, from, to), ct);
		return File(result.Content, result.ContentType, result.FileName);
	}

	[HttpGet("export/tasks/excel")]
	public async Task<IActionResult> ExportTasksExcel([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct = default(CancellationToken))
	{
		ExportResult result = await exportService.ExportTasksExcelAsync(tenantContext.TenantId, new ReportFilter(teamId, projectId, assigneeId, from, to), ct);
		return File(result.Content, result.ContentType, result.FileName);
	}

	[HttpGet("export/summary/pdf")]
	public async Task<IActionResult> ExportSummaryPdf([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct = default(CancellationToken))
	{
		ExportResult result = await exportService.ExportReportPdfAsync(tenantContext.TenantId, new ReportFilter(teamId, projectId, assigneeId, from, to), ct);
		return File(result.Content, result.ContentType, result.FileName);
	}
}
