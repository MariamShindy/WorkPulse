import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedList, SAVED_VIEW_ENTITY_TYPES, SavedView, enumValue } from '../models';

export interface SavedViewPayload {
  name: string;
  entityType: string;
  filtersJson: string;
  sortJson: string;
  isShared: boolean;
}

@Injectable({ providedIn: 'root' })
export class SavedViewsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/saved-views`;

  list(entityType?: string, page = 1, pageSize = 50): Observable<PagedList<SavedView>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (entityType) params = params.set('entityType', entityType);
    return this.http.get<PagedList<SavedView>>(this.base, { params });
  }

  get(savedViewId: string): Observable<SavedView> {
    return this.http.get<SavedView>(`${this.base}/${savedViewId}`);
  }

  create(payload: SavedViewPayload): Observable<SavedView> {
    return this.http.post<SavedView>(this.base, this.toBody(payload));
  }

  update(savedViewId: string, payload: SavedViewPayload): Observable<SavedView> {
    return this.http.put<SavedView>(`${this.base}/${savedViewId}`, this.toBody(payload));
  }

  delete(savedViewId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${savedViewId}`);
  }

  private toBody(payload: SavedViewPayload) {
    return {
      name: payload.name,
      entityType: enumValue(SAVED_VIEW_ENTITY_TYPES, payload.entityType),
      filtersJson: payload.filtersJson,
      sortJson: payload.sortJson,
      isShared: payload.isShared
    };
  }
}
