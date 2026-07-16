import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportFilter, ReportsService } from '../../core/services/reports.service';
import { TeamsService } from '../../core/services/teams.service';
import { ProjectsService } from '../../core/services/projects.service';
import {
  OverdueTasksReport,
  Project,
  TaskSummaryReport,
  Team,
  TeamPerformanceReport
} from '../../core/models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  private readonly reports = inject(ReportsService);
  private readonly teamsService = inject(TeamsService);
  private readonly projectsService = inject(ProjectsService);

  readonly teams = signal<Team[]>([]);
  readonly projects = signal<Project[]>([]);
  readonly loading = signal(true);
  readonly exporting = signal<string | null>(null);
  readonly error = signal<string | null>(null);

  readonly filterTeamId = signal('');
  readonly filterProjectId = signal('');
  readonly filterFrom = signal('');
  readonly filterTo = signal('');

  readonly summary = signal<TaskSummaryReport | null>(null);
  readonly overdue = signal<OverdueTasksReport | null>(null);
  readonly performance = signal<TeamPerformanceReport | null>(null);

  ngOnInit(): void {
    this.teamsService.list().subscribe({ next: (res) => this.teams.set(res.items) });
    this.projectsService.list({ pageSize: 100 }).subscribe({
      next: (res) => this.projects.set(res.items)
    });
    this.load();
  }

  setFilter(key: 'team' | 'project' | 'from' | 'to', value: string): void {
    if (key === 'team') this.filterTeamId.set(value);
    if (key === 'project') this.filterProjectId.set(value);
    if (key === 'from') this.filterFrom.set(value);
    if (key === 'to') this.filterTo.set(value);
    this.load();
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
        this.error.set(err.error?.description ?? 'Failed to load reports.');
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

  private currentFilter(): ReportFilter {
    return {
      teamId: this.filterTeamId() || undefined,
      projectId: this.filterProjectId() || undefined,
      from: this.filterFrom() || undefined,
      to: this.filterTo() || undefined
    };
  }
}
