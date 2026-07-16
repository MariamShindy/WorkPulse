import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedList, WorkLog } from '../models';

@Injectable({ providedIn: 'root' })
export class WorkLogsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/work-logs`;

  list(taskId: string, page = 1, pageSize = 25): Observable<PagedList<WorkLog>> {
    const params = new HttpParams().set('taskId', taskId).set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<WorkLog>>(this.base, { params });
  }

  create(taskId: string, hours: number, loggedDate: string, description?: string): Observable<WorkLog> {
    return this.http.post<WorkLog>(this.base, {
      taskId,
      hours,
      description: description ?? null,
      loggedDate
    });
  }

  delete(workLogId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${workLogId}`);
  }
}
