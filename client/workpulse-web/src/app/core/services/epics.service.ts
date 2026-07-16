import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { EPIC_STATUSES, Epic, PagedList, enumValue } from '../models';

@Injectable({ providedIn: 'root' })
export class EpicsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/epics`;

  list(filters: { teamId?: string; status?: string; page?: number; pageSize?: number } = {}): Observable<PagedList<Epic>> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 50);
    if (filters.teamId) params = params.set('teamId', filters.teamId);
    if (filters.status) params = params.set('status', filters.status);
    return this.http.get<PagedList<Epic>>(this.base, { params });
  }

  get(epicId: string): Observable<Epic> {
    return this.http.get<Epic>(`${this.base}/${epicId}`);
  }

  create(teamId: string, title: string, description: string | null, status: string): Observable<Epic> {
    return this.http.post<Epic>(this.base, {
      teamId,
      title,
      description,
      status: enumValue(EPIC_STATUSES, status)
    });
  }

  update(epicId: string, title: string, description: string | null, status: string): Observable<Epic> {
    return this.http.put<Epic>(`${this.base}/${epicId}`, {
      title,
      description,
      status: enumValue(EPIC_STATUSES, status)
    });
  }

  delete(epicId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${epicId}`);
  }
}
