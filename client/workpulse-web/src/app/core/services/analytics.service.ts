import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AssigneeWorkload,
  CycleTimeAnalytics,
  DashboardAnalytics,
  ProjectProgress,
  SprintBurndown,
  TeamVelocityPoint,
  ThroughputPoint
} from '../models';

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/analytics`;

  dashboard(filters: { teamId?: string; from?: string; to?: string } = {}): Observable<DashboardAnalytics> {
    let params = new HttpParams();
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.from) params = params.set('from', filters.from);
    if (filters.to) params = params.set('to', filters.to);
    return this.http.get<DashboardAnalytics>(`${this.base}/dashboard`, { params });
  }

  velocity(teamId?: string, weeks = 12): Observable<TeamVelocityPoint[]> {
    let params = new HttpParams().set('weeks', weeks);
    if (teamId) params = params.set('teamId', teamId);
    return this.http.get<TeamVelocityPoint[]>(`${this.base}/velocity`, { params });
  }

  workload(teamId?: string): Observable<AssigneeWorkload[]> {
    let params = new HttpParams();
    if (teamId) params = params.set('teamId', teamId);
    return this.http.get<AssigneeWorkload[]>(`${this.base}/workload`, { params });
  }

  projectProgress(teamId?: string): Observable<ProjectProgress[]> {
    let params = new HttpParams();
    if (teamId) params = params.set('teamId', teamId);
    return this.http.get<ProjectProgress[]>(`${this.base}/project-progress`, { params });
  }

  cycleTime(filters: { teamId?: string; from?: string; to?: string } = {}): Observable<CycleTimeAnalytics> {
    let params = new HttpParams();
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.from) params = params.set('from', filters.from);
    if (filters.to) params = params.set('to', filters.to);
    return this.http.get<CycleTimeAnalytics>(`${this.base}/cycle-time`, { params });
  }

  throughput(teamId?: string, granularity: 'Day' | 'Week' = 'Week', periods = 12): Observable<ThroughputPoint[]> {
    let params = new HttpParams().set('granularity', granularity).set('periods', periods);
    if (teamId) params = params.set('teamId', teamId);
    return this.http.get<ThroughputPoint[]>(`${this.base}/throughput`, { params });
  }

  sprintBurndown(sprintId: string): Observable<SprintBurndown> {
    return this.http.get<SprintBurndown>(`${this.base}/sprints/${sprintId}/burndown`);
  }
}
