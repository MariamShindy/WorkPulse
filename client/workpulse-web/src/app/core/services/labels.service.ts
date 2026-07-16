import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Label, PagedList } from '../models';

@Injectable({ providedIn: 'root' })
export class LabelsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/labels`;

  list(page = 1, pageSize = 100): Observable<PagedList<Label>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedList<Label>>(this.base, { params });
  }

  create(name: string, color: string): Observable<Label> {
    return this.http.post<Label>(this.base, { name, color });
  }

  update(labelId: string, name: string, color: string): Observable<Label> {
    return this.http.put<Label>(`${this.base}/${labelId}`, { name, color });
  }

  delete(labelId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${labelId}`);
  }
}
