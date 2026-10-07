import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PROJECT_STATUSES, PagedList, Project, enumValue } from '../models';

export interface ProjectFilters {
  teamId?: string;
  status?: string;
  includeArchived?: boolean;
  archivedOnly?: boolean;
  page?: number;
  pageSize?: number;
}

export interface ProjectPayload {
  name: string;
  status: string;
  key?: string | null;
  description?: string | null;
  leadId?: string | null;
  startDate?: string | null;
  targetDate?: string | null;
}

@Injectable({ providedIn: 'root' })
export class ProjectsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/projects`;

  list(filters: ProjectFilters = {}): Observable<PagedList<Project>> {
    const includeArchived = !!(filters.includeArchived || filters.archivedOnly);
    let params = new HttpParams()
      .set('page', String(filters.page ?? 1))
      .set('pageSize', String(filters.pageSize ?? 50))
      .set('includeArchived', String(includeArchived))
      .set('archivedOnly', String(!!filters.archivedOnly));
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.status) {
      params = params.set('status', String(enumValue(PROJECT_STATUSES, filters.status)));
    }
    return this.http.get<PagedList<Project>>(this.base, { params });
  }

  get(projectId: string): Observable<Project> {
    return this.http.get<Project>(`${this.base}/${projectId}`);
  }

  create(teamId: string, payload: ProjectPayload): Observable<Project> {
    return this.http.post<Project>(this.base, {
      teamId,
      name: payload.name,
      key: payload.key ?? null,
      description: payload.description ?? null,
      status: enumValue(PROJECT_STATUSES, payload.status),
      leadId: payload.leadId ?? null,
      startDate: payload.startDate ?? null,
      targetDate: payload.targetDate ?? null
    });
  }

  update(projectId: string, payload: ProjectPayload): Observable<Project> {
    return this.http.put<Project>(`${this.base}/${projectId}`, {
      name: payload.name,
      description: payload.description ?? null,
      status: enumValue(PROJECT_STATUSES, payload.status),
      leadId: payload.leadId ?? null,
      startDate: payload.startDate ?? null,
      targetDate: payload.targetDate ?? null
    });
  }

  archive(projectId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${projectId}/archive`, {});
  }
}
