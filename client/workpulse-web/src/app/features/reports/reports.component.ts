import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { ReportFilter, ReportsService } from '../../core/services/reports.service';
import { TeamsService } from '../../core/services/teams.service';
import { ProjectsService } from '../../core/services/projects.service';
import { AnalyticsService } from '../../core/services/analytics.service';
import { SprintsService } from '../../core/services/sprints.service';
import {
  CycleTimeAnalytics,
  OverdueTasksReport,
  Project,
  Sprint,
  SprintBurndown,
  TaskSummaryReport,
  Team,
  TeamPerformanceReport,
  ThroughputPoint
} from '../../core/models';
import { apiErrorMessage } from '../../core/utils/api-error';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  private readonly reports = inject(ReportsService);
  private readonly teamsService = inject(TeamsService);
  private readonly projectsService = inject(ProjectsService);
  private readonly analytics = inject(AnalyticsService);
  private readonly sprintsService = inject(SprintsService);

  readonly teams = signal<Team[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly loading = signal(true);
  readonly exporting = signal<string | null>(null);
  readonly analyticsExporting = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  readonly filterTeamId = signal('');
  readonly filterProjectId = signal('');
  readonly filterFrom = signal('');
  readonly filterTo = signal('');

  readonly summary = signal<TaskSummaryReport | null>(null);
  readonly overdue = signal<OverdueTasksReport | null>(null);
  readonly performance = signal<TeamPerformanceReport | null>(null);

  readonly cycleTime = signal<CycleTimeAnalytics | null>(null);
  readonly throughput = signal<ThroughputPoint[]>([]);

  readonly sprints = signal<Sprint[]>([]);
  readonly selectedSprintId = signal('');
  readonly burndown = signal<SprintBurndown | null>(null);
  readonly burndownLoading = signal(false);

  readonly pageSize = 10;
  readonly currentPage = signal(1);

  readonly paginatedSummaryRows = computed(() => {
    const s = this.summary();
    if (!s) return [];
    const start = (this.currentPage() - 1) * this.pageSize;
    return s.rows.slice(start, start + this.pageSize);
  });

  readonly totalPages = computed(() => {
    const s = this.summary();
    if (!s) return 0;
    return Math.ceil(s.rows.length / this.pageSize);
  });

  readonly chartOptions: ChartOptions = { responsive: true, maintainAspectRatio: false };

  readonly histogramChartData = computed<ChartConfiguration<'bar'>['data']>(() => {
    const histogram = this.cycleTime()?.histogram ?? [];
    return {
      labels: histogram.map((bucket) => bucket.label),
      datasets: [{ label: 'Tasks', data: histogram.map((bucket) => bucket.count), backgroundColor: '#6366f1' }]
    };
  });

  readonly throughputChartData = computed<ChartConfiguration<'bar'>['data']>(() => {
    const points = this.throughput();
    return {
      labels: points.map((point) => point.periodStart),
      datasets: [
        { label: 'Completed tasks', data: points.map((point) => point.completedTasks), backgroundColor: '#22c55e' },
        { label: 'Completed points', data: points.map((point) => point.completedStoryPoints), backgroundColor: '#0ea5e9' }
      ]
    };
  });

  readonly burndownChartData = computed<ChartConfiguration<'line'>['data']>(() => {
    const points = this.burndown()?.points ?? [];
    return {
      labels: points.map((point) => point.date),
      datasets: [
        { label: 'Remaining', data: points.map((point) => point.remaining), borderColor: '#ef4444', backgroundColor: 'transparent', tension: 0.2 },
        { label: 'Ideal', data: points.map((point) => point.idealRemaining), borderColor: '#94a3b8', borderDash: [6, 4], backgroundColor: 'transparent', pointRadius: 0 },
        { label: 'Completed (burnup)', data: points.map((point) => point.completedCumulative), borderColor: '#22c55e', backgroundColor: 'transparent', tension: 0.2 }
      ]
    };
  });

  ngOnInit(): void {
    this.teamsService.list().subscribe({ next: (res) => this.teams.set(res.items) });
    this.projectsService.list({ pageSize: 100 }).subscribe({
      next: (res) => this.projects.set(res.items)
    });
    this.load();
    this.loadAnalytics();
    this.loadSprints();
  }

  setFilter(key: 'team' | 'project' | 'from' | 'to', value: string): void {
    if (key === 'team') this.filterTeamId.set(value);
    if (key === 'project') this.filterProjectId.set(value);
    if (key === 'from') this.filterFrom.set(value);
    if (key === 'to') this.filterTo.set(value);
    this.currentPage.set(1);
    this.load();
    this.loadAnalytics();
    if (key === 'team') {
      this.loadSprints();
    }
  }

  setPage(page: number): void {
    this.currentPage.set(page);
  }

  setSelectedSprint(sprintId: string): void {
    this.selectedSprintId.set(sprintId);
    this.loadBurndown();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    const filter = this.currentFilter();

    this.reports.taskSummary(filter).subscribe({
      next: (report) => {
        this.summary.set(report);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Failed to load reports.'));
        this.loading.set(false);
      }
    });
    this.reports.overdueTasks(filter).subscribe({
      next: (report) => this.overdue.set(report)
    });
    this.reports.teamPerformance(filter).subscribe({
      next: (report) => this.performance.set(report)
    });
  }

  export(kind: 'csv' | 'excel' | 'pdf'): void {
    this.exporting.set(kind);
    const filter = this.currentFilter();
    const request$ =
      kind === 'csv'
        ? this.reports.exportTasksCsv(filter)
        : kind === 'excel'
          ? this.reports.exportTasksExcel(filter)
          : this.reports.exportSummaryPdf(filter);

    const fallback =
      kind === 'csv' ? 'tasks.csv' : kind === 'excel' ? 'tasks.xlsx' : 'workpulse-report.pdf';

    request$.subscribe({
      next: (response) => {
        this.reports.downloadBlob(response, fallback);
        this.exporting.set(null);
      },
      error: () => {
        this.error.set(`Failed to export ${kind.toUpperCase()}.`);
        this.exporting.set(null);
      }
    });
  }

  exportAnalytics(kind: 'cycle-csv' | 'summary-pdf'): void {
    this.analyticsExporting.set(kind);
    const filter = this.currentFilter();
    const request$ =
      kind === 'cycle-csv'
        ? this.reports.exportCycleTimeCsv(filter)
        : this.reports.exportAnalyticsSummaryPdf(filter);
    const fallback = kind === 'cycle-csv' ? 'cycle-time.csv' : 'workpulse-analytics-summary.pdf';

    request$.subscribe({
      next: (response) => {
        this.reports.downloadBlob(response, fallback);
        this.analyticsExporting.set(null);
      },
      error: () => {
        this.error.set(`Failed to export ${kind === 'cycle-csv' ? 'cycle time CSV' : 'analytics summary PDF'}.`);
        this.analyticsExporting.set(null);
      }
    });
  }

  private loadAnalytics(): void {
    const filter = this.currentFilter();
    this.analytics.cycleTime({ teamId: filter.teamId, from: filter.from, to: filter.to }).subscribe({
      next: (data) => this.cycleTime.set(data)
    });
    this.analytics.throughput(filter.teamId, 'Week', 12).subscribe({
      next: (data) => this.throughput.set(data)
    });
  }

  private loadSprints(): void {
    const teamId = this.filterTeamId() || undefined;
    this.sprintsService.list({ teamId, pageSize: 100 }).subscribe({
      next: (res) => {
        this.sprints.set(res.items);
        const stillValid = res.items.some((sprint) => sprint.id === this.selectedSprintId());
        if (!stillValid) {
          this.selectedSprintId.set(res.items[0]?.id ?? '');
        }
        this.loadBurndown();
      }
    });
  }

  private loadBurndown(): void {
    const sprintId = this.selectedSprintId();
    if (!sprintId) {
      this.burndown.set(null);
      return;
    }
    this.burndownLoading.set(true);
    this.analytics.sprintBurndown(sprintId).subscribe({
      next: (data) => {
        this.burndown.set(data);
        this.burndownLoading.set(false);
      },
      error: () => {
        this.burndown.set(null);
        this.burndownLoading.set(false);
      }
    });
  }

  private currentFilter(): ReportFilter {
    return {
      teamId: this.filterTeamId() || undefined,
      projectId: this.filterProjectId() || undefined,
      from: this.filterFrom() || undefined,
      to: this.filterTo() || undefined
    };
  }
}
