using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Infrastructure.Services;

internal static class ReportPdfGenerator
{
	static ReportPdfGenerator()
	{
		QuestPDF.Settings.License = LicenseType.Community;
	}

	public static byte[] Generate(
		TaskSummaryReportDto summary,
		OverdueTasksReportDto overdue,
		TeamPerformanceReportDto performance)
	{
		return Document.Create(container =>
		{
			container.Page(page =>
			{
				page.Size(PageSizes.A4);
				page.MarginHorizontal(36);
				page.MarginVertical(36);
				page.DefaultTextStyle(x => x.FontSize(9));

				page.Header().Column(col =>
				{
					col.Item().Text("WorkPulse Report").Bold().FontSize(18);
					col.Item().Text($"Generated {summary.GeneratedAtUtc:yyyy-MM-dd HH:mm} UTC")
						.FontColor(Colors.Grey.Darken1);
				});

				page.Content().PaddingTop(12).Column(col =>
				{
					col.Item().SectionTitle("Summary");
					col.Item().PaddingTop(4).Row(row =>
					{
						StatCell(row, "Total", summary.TotalTasks);
						StatCell(row, "Open", summary.OpenTasks);
						StatCell(row, "Completed", summary.CompletedTasks);
						StatCell(row, "Overdue", summary.OverdueTasks);
					});

					if (summary.Rows.Count > 0)
					{
						col.Item().PaddingTop(16).SectionTitle("Task summary");
						col.Item().PaddingTop(4).Element(c => RenderTaskTable(c, summary.Rows));
					}
					else
					{
						col.Item().PaddingTop(16).Text("No tasks match the current filters.").Italic();
					}

					col.Item().PaddingTop(16).SectionTitle($"Overdue tasks ({overdue.TotalOverdue})");
					if (overdue.Rows.Count > 0)
					{
						col.Item().PaddingTop(4).Element(c => RenderTaskTable(c, overdue.Rows, includeDueDate: true));
					}
					else
					{
						col.Item().PaddingTop(4).Text("Nothing overdue.").Italic();
					}

					col.Item().PaddingTop(16).SectionTitle("Team performance");
					if (performance.Teams.Count > 0)
					{
						col.Item().PaddingTop(4).Element(RenderTeamPerformanceTable);
					}
					else
					{
						col.Item().PaddingTop(4).Text("No team data available.").Italic();
					}

					void RenderTeamPerformanceTable(IContainer container)
					{
						container.Table(table =>
						{
							table.ColumnsDefinition(columns =>
							{
								columns.RelativeColumn(2);
								columns.ConstantColumn(42);
								columns.ConstantColumn(52);
								columns.ConstantColumn(48);
								columns.ConstantColumn(58);
								columns.ConstantColumn(58);
							});
							table.Header(header =>
							{
								HeaderCell(header, "Team");
								HeaderCell(header, "Total");
								HeaderCell(header, "Completed");
								HeaderCell(header, "Overdue");
								HeaderCell(header, "Completion");
								HeaderCell(header, "Avg cycle");
							});
							foreach (TeamPerformanceRowDto team in performance.Teams)
							{
								BodyCell(table, $"{team.TeamName} ({team.TeamKey})");
								BodyCell(table, team.TotalTasks.ToString(CultureInfo.InvariantCulture));
								BodyCell(table, team.CompletedTasks.ToString(CultureInfo.InvariantCulture));
								BodyCell(table, team.OverdueTasks.ToString(CultureInfo.InvariantCulture));
								BodyCell(table, $"{team.CompletionRate * 100:0}%");
								BodyCell(table, $"{team.AverageCycleTimeDays:0.#} d");
							}
						});
					}
				});

				page.Footer().AlignCenter().Text(text =>
				{
					text.Span("Page ");
					text.CurrentPageNumber();
					text.Span(" of ");
					text.TotalPages();
				});
			});
		}).GeneratePdf();
	}

	private static void StatCell(RowDescriptor row, string label, int value)
	{
		row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
		{
			col.Item().Text(label).FontColor(Colors.Grey.Darken1).FontSize(8);
			col.Item().Text(value.ToString(CultureInfo.InvariantCulture)).Bold().FontSize(14);
		});
	}

	private static void RenderTaskTable(IContainer container, IReadOnlyList<ReportTaskRowDto> rows, bool includeDueDate = true)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.ConstantColumn(52);
				columns.RelativeColumn(3);
				columns.ConstantColumn(36);
				columns.ConstantColumn(44);
				columns.ConstantColumn(48);
				columns.ConstantColumn(48);
				if (includeDueDate)
				{
					columns.ConstantColumn(52);
				}
			});
			table.Header(header =>
			{
				HeaderCell(header, "ID");
				HeaderCell(header, "Title");
				HeaderCell(header, "Team");
				HeaderCell(header, "Project");
				HeaderCell(header, "Status");
				HeaderCell(header, "Priority");
				if (includeDueDate)
				{
					HeaderCell(header, "Due");
				}
			});
			foreach (ReportTaskRowDto row in rows)
			{
				BodyCell(table, row.Identifier);
				BodyCell(table, row.Title);
				BodyCell(table, row.TeamKey);
				BodyCell(table, row.ProjectKey ?? "—");
				BodyCell(table, row.Status);
				BodyCell(table, row.Priority);
				if (includeDueDate)
				{
					BodyCell(table, row.DueDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—");
				}
			}
		});
	}

	private static void HeaderCell(TableCellDescriptor header, string text)
	{
		header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text(text).Bold();
	}

	private static void BodyCell(TableDescriptor table, string text)
	{
		table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(text);
	}

	private static void SectionTitle(this IContainer container, string title)
	{
		container.Text(title).Bold().FontSize(12);
	}
}
