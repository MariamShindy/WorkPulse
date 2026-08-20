using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ClosedXML.Excel;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Infrastructure.Services;

public sealed class ExportService(IApplicationDbContext context, IReportsReadService reports) : IExportService
{
	public async Task<ExportResult> ExportTasksCsvAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken))
	{
		IReadOnlyList<ReportTaskRowDto> rows = await GetTaskRowsAsync(tenantId, filter, ct);
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("Identifier,Title,Team,Project,Status,Priority,AssigneeId,DueDate,CreatedAt");
		foreach (ReportTaskRowDto row in rows)
		{
			InlineArray9<string> buffer = default(InlineArray9<string>);
			buffer[0] = Escape(row.Identifier);
			buffer[1] = Escape(row.Title);
			buffer[2] = Escape(row.TeamKey);
			buffer[3] = Escape(row.ProjectKey);
			buffer[4] = Escape(row.Status);
			buffer[5] = Escape(row.Priority);
			buffer[6] = row.AssigneeId?.ToString() ?? string.Empty;
			buffer[7] = row.DueDate?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty;
			buffer[8] = row.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture);
			sb.AppendLine(string.Join(',', (ReadOnlySpan<string>)buffer));
		}
		return new ExportResult(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "tasks.csv");
	}

	public async Task<ExportResult> ExportTasksExcelAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken))
	{
		IReadOnlyList<ReportTaskRowDto> rows = await GetTaskRowsAsync(tenantId, filter, ct);
		using XLWorkbook workbook = new XLWorkbook();
		IXLWorksheet sheet = workbook.Worksheets.Add("Tasks");
		sheet.Cell(1, 1).Value = "Identifier";
		sheet.Cell(1, 2).Value = "Title";
		sheet.Cell(1, 3).Value = "Team";
		sheet.Cell(1, 4).Value = "Project";
		sheet.Cell(1, 5).Value = "Status";
		sheet.Cell(1, 6).Value = "Priority";
		sheet.Cell(1, 7).Value = "AssigneeId";
		sheet.Cell(1, 8).Value = "DueDate";
		sheet.Cell(1, 9).Value = "CreatedAt";
		int rowIndex = 2;
		foreach (ReportTaskRowDto row in rows)
		{
			sheet.Cell(rowIndex, 1).Value = row.Identifier;
			sheet.Cell(rowIndex, 2).Value = row.Title;
			sheet.Cell(rowIndex, 3).Value = row.TeamKey;
			sheet.Cell(rowIndex, 4).Value = row.ProjectKey;
			sheet.Cell(rowIndex, 5).Value = row.Status;
			sheet.Cell(rowIndex, 6).Value = row.Priority;
			sheet.Cell(rowIndex, 7).Value = row.AssigneeId?.ToString();
			sheet.Cell(rowIndex, 8).Value = row.DueDate?.ToString("O");
			sheet.Cell(rowIndex, 9).Value = row.CreatedAtUtc.ToString("O");
			rowIndex++;
		}
		sheet.Columns().AdjustToContents();
		using MemoryStream stream = new MemoryStream();
		workbook.SaveAs(stream);
		return new ExportResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "tasks.xlsx");
	}

	public async Task<ExportResult> ExportReportPdfAsync(Guid tenantId, ReportFilter filter, CancellationToken ct = default(CancellationToken))
	{
		TaskSummaryReportDto summary = await reports.GetTaskSummaryReportAsync(tenantId, filter, ct);
		OverdueTasksReportDto overdue = await reports.GetOverdueTasksReportAsync(tenantId, filter, ct);
		TeamPerformanceReportDto performance = await reports.GetTeamPerformanceReportAsync(tenantId, filter, ct);
		byte[] pdf = ReportPdfGenerator.Generate(summary, overdue, performance);
		return new ExportResult(pdf, "application/pdf", "workpulse-report.pdf");
	}

	private async Task<IReadOnlyList<ReportTaskRowDto>> GetTaskRowsAsync(Guid tenantId, ReportFilter filter, CancellationToken ct)
	{
		TaskSummaryReportDto summary = await reports.GetTaskSummaryReportAsync(tenantId, filter, ct);
		if (summary.Rows.Count > 0)
		{
			return summary.Rows;
		}
		IQueryable<ReportTaskRowDto> query = from t in context.TaskItems.AsNoTracking()
			join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking() on t.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			where t.TenantId == tenantId
			select new ReportTaskRowDto(t.Id, string.Concat(team.Key + "-", t.Number), t.Title, team.Key, (project != null) ? project.Key : null, state.Name, t.Priority.ToString(), t.AssigneeId, t.DueDate, t.CreatedAtUtc);
		if (filter.TeamId.HasValue)
		{
			query = query.Where((ReportTaskRowDto r) => context.TaskItems.Any((TaskItem t) => t.Id == r.TaskId && t.TeamId == filter.TeamId));
		}
		return await query.ToListAsync(ct);
	}

	private static string Escape(string? value)
	{
		if (value == null)
		{
			value = string.Empty;
		}
		return (value.Contains(',') || value.Contains('"')) ? ("\"" + value.Replace("\"", "\"\"") + "\"") : value;
	}
}
