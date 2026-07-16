import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuditLogEntry, PagedList } from '../models';

@Injectable({ providedIn: 'root' })
export class AuditLogsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/audit-logs`;

  list(
    filters: { entityType?: string; entityId?: string; userId?: string; page?: number; pageSize?: number } = {}
  ): Observable<PagedList<AuditLogEntry>> {
    let params = new HttpParams()
      .set('page', filters.page ?? 1)
      .set('pageSize', filters.pageSize ?? 25);
    if (filters.entityType) params = params.set('entityType', filters.entityType);
    if (filters.entityId) params = params.set('entityId', filters.entityId);
    if (filters.userId) params = params.set('userId', filters.userId);
    return this.http.get<PagedList<AuditLogEntry>>(this.base, { params });
  }
}
