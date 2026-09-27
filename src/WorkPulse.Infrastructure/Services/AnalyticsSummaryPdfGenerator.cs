using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WorkPulse.Application.Analytics.Dtos;

namespace WorkPulse.Infrastructure.Services;

internal static class AnalyticsSummaryPdfGenerator
{
	static AnalyticsSummaryPdfGenerator()
	{
		QuestPDF.Settings.License = LicenseType.Community;
	}

	public static byte[] Generate(
		IReadOnlyList<TeamVelocityPointDto> velocity,
		CycleTimeAnalyticsDto cycleTime,
		IReadOnlyList<ThroughputPointDto> throughput)
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
					col.Item().Text("WorkPulse Analytics Summary").Bold().FontSize(18);
					col.Item().Text($"Generated {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontColor(Colors.Grey.Darken1);
				});

				page.Content().PaddingTop(12).Column(col =>
				{
					col.Item().SectionTitle("Cycle time & lead time");
					col.Item().PaddingTop(4).Row(row =>
					{
						StatCell(row, "Sample size", cycleTime.SampleSize.ToString(CultureInfo.InvariantCulture));
						StatCell(row, "Avg cycle", $"{cycleTime.AverageCycleTimeDays:0.#} d");
						StatCell(row, "Median cycle", $"{cycleTime.MedianCycleTimeDays:0.#} d");
						StatCell(row, "P85 cycle", $"{cycleTime.P85CycleTimeDays:0.#} d");
						StatCell(row, "Avg lead time", $"{cycleTime.AverageLeadTimeDays:0.#} d");
					});

					col.Item().PaddingTop(16).SectionTitle("Cycle time distribution");
					col.Item().PaddingTop(4).Element(c => RenderHistogram(c, cycleTime.Histogram));

					col.Item().PaddingTop(16).SectionTitle("Weekly velocity (created vs completed)");
					col.Item().PaddingTop(4).Element(c => RenderVelocityTable(c, velocity));

					col.Item().PaddingTop(16).SectionTitle("Weekly throughput");
					col.Item().PaddingTop(4).Element(c => RenderThroughputTable(c, throughput));
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

	private static void StatCell(RowDescriptor row, string label, string value)
	{
		row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(col =>
		{
			col.Item().Text(label).FontColor(Colors.Grey.Darken1).FontSize(8);
			col.Item().Text(value).Bold().FontSize(13);
		});
	}

	private static void RenderHistogram(IContainer container, IReadOnlyList<CycleTimeBucketDto> buckets)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.RelativeColumn(2);
				columns.RelativeColumn(1);
			});
			table.Header(header =>
			{
				ReportPdfGenerator.HeaderCell(header, "Cycle time");
				ReportPdfGenerator.HeaderCell(header, "Tasks");
			});
			foreach (CycleTimeBucketDto bucket in buckets)
			{
				ReportPdfGenerator.BodyCell(table, bucket.Label);
				ReportPdfGenerator.BodyCell(table, bucket.Count.ToString(CultureInfo.InvariantCulture));
			}
		});
	}

	private static void RenderVelocityTable(IContainer container, IReadOnlyList<TeamVelocityPointDto> velocity)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.RelativeColumn(2);
				columns.RelativeColumn(1);
				columns.RelativeColumn(1);
			});
			table.Header(header =>
			{
				ReportPdfGenerator.HeaderCell(header, "Week of");
				ReportPdfGenerator.HeaderCell(header, "Created");
				ReportPdfGenerator.HeaderCell(header, "Completed");
			});
			foreach (TeamVelocityPointDto point in velocity)
			{
				ReportPdfGenerator.BodyCell(table, point.WeekStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
				ReportPdfGenerator.BodyCell(table, point.Created.ToString(CultureInfo.InvariantCulture));
				ReportPdfGenerator.BodyCell(table, point.Completed.ToString(CultureInfo.InvariantCulture));
			}
		});
	}

	private static void RenderThroughputTable(IContainer container, IReadOnlyList<ThroughputPointDto> throughput)
	{
		container.Table(table =>
		{
			table.ColumnsDefinition(columns =>
			{
				columns.RelativeColumn(2);
				columns.RelativeColumn(1);
				columns.RelativeColumn(1);
			});
			table.Header(header =>
			{
				ReportPdfGenerator.HeaderCell(header, "Week of");
				ReportPdfGenerator.HeaderCell(header, "Completed tasks");
				ReportPdfGenerator.HeaderCell(header, "Completed points");
			});
			foreach (ThroughputPointDto point in throughput)
			{
				ReportPdfGenerator.BodyCell(table, point.PeriodStart.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
				ReportPdfGenerator.BodyCell(table, point.CompletedTasks.ToString(CultureInfo.InvariantCulture));
				ReportPdfGenerator.BodyCell(table, point.CompletedStoryPoints.ToString(CultureInfo.InvariantCulture));
			}
		});
	}
}
