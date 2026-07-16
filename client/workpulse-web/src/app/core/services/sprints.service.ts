import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedList, SPRINT_STATUSES, Sprint, TaskItem, enumValue } from '../models';

export interface SprintPayload {
  name: string;
  goal?: string | null;
  startDate: string;
  endDate: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class SprintsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/sprints`;

  list(filters: { teamId?: string; status?: string; page?: number; pageSize?: number } = {}): Observable<PagedList<Sprint>> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 50);
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.status) params = params.set('status', filters.status);
    return this.http.get<PagedList<Sprint>>(this.base, { params });
  }

  get(sprintId: string): Observable<Sprint> {
    return this.http.get<Sprint>(`${this.base}/${sprintId}`);
  }

  backlog(teamId: string, sprintId?: string, page = 1, pageSize = 50): Observable<PagedList<TaskItem>> {
    const params = new HttpParams().set('teamId', teamId).set('page', page).set('pageSize', pageSize);
    const url = sprintId ? `${this.base}/${sprintId}/backlog` : `${this.base}/backlog`;
    return this.http.get<PagedList<TaskItem>>(url, { params });
  }

  create(teamId: string, payload: SprintPayload): Observable<Sprint> {
    return this.http.post<Sprint>(this.base, {
      teamId,
      name: payload.name,
      goal: payload.goal ?? null,
      startDate: payload.startDate,
      endDate: payload.endDate,
      status: enumValue(SPRINT_STATUSES, payload.status)
    });
  }

  update(sprintId: string, payload: SprintPayload): Observable<Sprint> {
    return this.http.put<Sprint>(`${this.base}/${sprintId}`, {
      name: payload.name,
      goal: payload.goal ?? null,
      startDate: payload.startDate,
      endDate: payload.endDate,
      status: enumValue(SPRINT_STATUSES, payload.status)
    });
  }

  delete(sprintId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${sprintId}`);
  }
}
