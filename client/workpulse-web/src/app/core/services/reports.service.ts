import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OverdueTasksReport, TaskSummaryReport, TeamPerformanceReport } from '../models';

export interface ReportFilter {
  teamId?: string;
  projectId?: string;
  assigneeId?: string;
  from?: string;
  to?: string;
}

@Injectable({ providedIn: 'root' })
export class ReportsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/reports`;

  taskSummary(filter: ReportFilter = {}): Observable<TaskSummaryReport> {
    return this.http.get<TaskSummaryReport>(`${this.base}/task-summary`, {
      params: this.toParams(filter)
    });
  }

  overdueTasks(filter: ReportFilter = {}): Observable<OverdueTasksReport> {
    return this.http.get<OverdueTasksReport>(`${this.base}/overdue-tasks`, {
      params: this.toParams(filter)
    });
  }

  teamPerformance(filter: ReportFilter = {}): Observable<TeamPerformanceReport> {
    return this.http.get<TeamPerformanceReport>(`${this.base}/team-performance`, {
      params: this.toParams(filter)
    });
  }

  exportTasksCsv(filter: ReportFilter = {}): Observable<HttpResponse<Blob>> {
    return this.export(`${this.base}/export/tasks/csv`, filter);
  }

  exportTasksExcel(filter: ReportFilter = {}): Observable<HttpResponse<Blob>> {
    return this.export(`${this.base}/export/tasks/excel`, filter);
  }

  exportSummaryPdf(filter: ReportFilter = {}): Observable<HttpResponse<Blob>> {
    return this.export(`${this.base}/export/summary/pdf`, filter);
  }

  /** Triggers a browser download for an export response. */
  downloadBlob(response: HttpResponse<Blob>, fallbackName: string): void {
    const disposition = response.headers.get('Content-Disposition') ?? '';
    const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition);
    const name = match ? decodeURIComponent(match[1]) : fallbackName;

    const contentType = response.headers.get('Content-Type') ?? 'application/octet-stream';
    const url = URL.createObjectURL(new Blob([response.body!], { type: contentType }));
    const link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    URL.revokeObjectURL(url);
  }

  private export(url: string, filter: ReportFilter): Observable<HttpResponse<Blob>> {
    return this.http.get(url, {
      params: this.toParams(filter),
      responseType: 'blob',
      observe: 'response'
    });
  }

  private toParams(filter: ReportFilter): HttpParams {
    let params = new HttpParams();
    if (filter.teamId) params = params.set('teamId', filter.teamId);
    if (filter.projectId) params = params.set('projectId', filter.projectId);
    if (filter.assigneeId) params = params.set('assigneeId', filter.assigneeId);
    if (filter.from) params = params.set('from', filter.from);
    if (filter.to) params = params.set('to', filter.to);
    return params;
  }
}
